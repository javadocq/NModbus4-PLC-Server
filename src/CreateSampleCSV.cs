using System;
using System.Collections.Generic;
using System.Text;

namespace NModbus4_PLC_Server.src
{
    public class CreateSampleCSV
    {
        public static void CreateCsvFile(string filePath)
        {
            if (File.Exists(filePath)) return;

            var csvContent = new StringBuilder();
            csvContent.AppendLine("Index,Temper,Pressure,Status");

            Random rand = new Random();

            // 초기값 설정 (시작점)
            double currentTemp = 250;   // 25.0도
            double currentPress = 1000; // 100.0 bar

            for (int i = 0; i < 1500; i++)
            {
                // 1. 온도: 이전 값에서 -1.5 ~ +1.5 사이로 변화 (연속성 확보)
                currentTemp += (rand.NextDouble() * 3) - 1.5;

                // 너무 낮아지거나 높아지지 않게 범위 제한 (20도 ~ 40도)
                if (currentTemp < 200) currentTemp = 200;
                if (currentTemp > 450) currentTemp = 450;

                // 2. 압력: 온도에 어느 정도 비례하면서도 독자적인 노이즈 추가
                // 온도가 오르면 압력도 살짝 오르는 경향을 반영
                double pressureNoise = (rand.NextDouble() * 10) - 5; // ±5 노이즈
                currentPress = (currentTemp * 4) + pressureNoise;

                // 3. 상태값: 온도가 38도(380)를 넘으면 에러(2) 발생 확률 증가
                int status = (currentTemp >= 380) ? 2 : 1;

                // 소수점 제거를 위해 정수로 변환하여 저장
                csvContent.AppendLine($"{i},{(int)currentTemp},{(int)currentPress},{status}");
            }

            File.WriteAllText(filePath, csvContent.ToString(), Encoding.UTF8);
            Console.WriteLine($"✔ 연관성 있는 시나리오 데이터 생성 완료!");
        }
    }
}

