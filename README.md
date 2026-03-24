# 🏭 NModbus4-PLC-Simulator

> **CSV 시계열 데이터를 기반으로 실시간 Modbus TCP 데이터를 생성하는 멀티 포트 가상 PLC 서버**

본 프로젝트는 스마트 팩토리 및 HMI/SCADA 시스템 개발 시, 실제 PLC 장비 없이도 **실제 공정 데이터(CSV)**를 활용해 통신 및 제어 로직을 테스트할 수 있도록 설계된 시뮬레이터입니다. .NET 환경에서 NModbus4 라이브러리를 활용하여 산업용 표준 프로토콜을 완벽히 모사합니다.

---

## 🚀 주요 특징 (Key Features)

* **Multi-Port Support**: 하나의 서버 인스턴스에서 여러 개의 TCP 포트(502, 503, 504...)를 독립적으로 가동하여 멀티 설비 환경 구축 가능.
* **Time-Series Data Playback**: `data.csv` 파일을 로드하여 실제 센서 데이터(온도, 압력 등)를 1초 단위로 레지스터에 업데이트하는 시뮬레이션 기능.
* **Bi-directional Control**: 상위 시스템(WPF, SCADA 등)으로부터의 Coil 쓰기 명령을 감지하여 설비의 가동/정지 로직을 실시간으로 반영.
* **Thread-Safe Architecture**: `Task.Run`을 이용한 비동기 리스닝과 `lock` 키워드를 활용한 데이터 무결성 보장.

---

## 🛠 기술 스택 (Tech Stack)

* **Framework**: .NET 10.0
* **Protocol**: Modbus TCP (via NModbus4)
* **Concurreny**: Task-based Asynchronous Pattern (TAP)
* **Data Format**: CSV (Time-series log)

---

## 📂 프로젝트 구조 (Project Structure)

```text
/
├── src/
│   ├── Program.cs           # 가상 PLC 서버 핵심 로직 (Multi-threaded)
│   └── FakePLCServer.csproj  # 프로젝트 설정 및 의존성 관리
├── sample_data/
│   └── data.csv             # 시뮬레이션용 센서 데이터 샘플 (1,500 rows)
└── README.md                # 프로젝트 가이드 및 문서화