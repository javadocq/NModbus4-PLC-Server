using Modbus.Data;
using Modbus.Device;
using System.Net.Sockets;

namespace NModbus4_PLC_Server.src
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("NModbus4 PLC Server is running...");

            // 현재 프로그램이 실행되는 폴더(bin/Debug/...)의 경로를 가져옵니다.
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // 그 폴더 안에 data.csv라는 이름을 붙입니다.
            string filePath = Path.Combine(baseDirectory, "data.csv");

            // 이제 이 filePath를 생성기와 서버 시작 함수에 전달하면 끝
            CreateSampleCSV.CreateCsvFile(filePath);

            // 머신 포트 번호 설정
            int[] ports = new int[] { 502, 503, 504, 505 };
            
            foreach(int port in ports) {
                Task.Run(() => startSlave(port, filePath)); // 각 기계마다 대기상태로 진입, 비동기로 실행하여 동시에 여러 기계가 연결될 수 있도록 함
            }

            Console.WriteLine("PLC 가상 서버 시작, 각 포트별로 머신 대기 중...");
            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();


        }

        public static void startSlave(int port, string csvFilePath)
        {
            try
            {
                TcpListener slaveTcpListener = new TcpListener(System.Net.IPAddress.Any, port);
                slaveTcpListener.Start(); // TCP 리스너 시작

                Console.WriteLine("Machine on port " + port + " is waiting for connection...");

                byte unitId = 1; // 유닛 ID 설정
                DataStore dataStore = DataStoreFactory.CreateDefaultDataStore(); // 데이터 저장소 생성
                ModbusSlave slave = ModbusTcpSlave.CreateTcp(unitId, slaveTcpListener); // Modbus TCP 슬레이브 생성
                slave.DataStore = dataStore; // 데이터 저장소 할당

                Console.WriteLine("Machine on port" + port + " is ready to accept Modbus requests...");

                dataStore.CoilDiscretes[1] = false; // 기계 상태를 설정 (true : 기계 가동 중, false : 기계 정지 중)

                
                Task.Run(() => slave.Listen()); // 슬레이브 시작, 비동기로 실행하여 동시에 여러 기계가 연결될 수 있도록 함
                Console.WriteLine($"[Port {port}] 가상 PLC 서버 시작. 연결 대기 중...");

                if (!File.Exists(csvFilePath))
                {
                    Console.WriteLine($"[Port {port}] {csvFilePath} 파일이 없습니다.");
                    return;
                }

                Task.Run(() =>
                {
                    while (true)
                    {
                        if (dataStore.CoilDiscretes[1])
                        {
                            Console.WriteLine("[Port " + port + "] Machine is running. Reading data from CSV...");
                            var lines = File.ReadAllLines(csvFilePath);
                            for (int i = 1; i < lines.Length; i++)
                            {
                                if (!dataStore.CoilDiscretes[1])
                                {
                                    Console.WriteLine("[Port " + port + "] Machine stopped. Stopping data reading...");
                                    break;
                                }

                                var parts = lines[i].Split(',');
                                if (parts.Length < 3) continue;

                                ushort Temper = ushort.Parse(parts[1]);
                                ushort Pressure = ushort.Parse(parts[2]);

                                lock (dataStore.SyncRoot)
                                {
                                    dataStore.HoldingRegisters[1] = Temper; // 온도 데이터 저장
                                    dataStore.HoldingRegisters[2] = Pressure; // 압력 데이터 저장
                                }

                                Console.WriteLine($"[Port {port}] Updated Holding Registers: Temperature={Temper}, Pressure={Pressure}");
                                Thread.Sleep(1000); // 1초마다 데이터 업데이트

                            }
                        }
                        else
                        {
                            // 정지 상태일 때는 CPU 점유율 낮추기 위해 대기
                            Thread.Sleep(500);
                        }
                    }
                });


            }
            catch (Exception ex) { 
                Console.WriteLine("[Port " +  port + "] : " + ex.ToString());
            }
        }
    }
}
