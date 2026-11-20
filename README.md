# MergeAlchemy
## 프로젝트 소개
 - 작품명 : **Merge Alchemy**
 - 장르 : **머지 게임**
 - 플랫폼 : `Android`
 - 개발 기간 : `2026.04~2026.11`
 - 개발 환경 : `Unity 6000.3.9f1 (urp)`
 - 개발 언어 : `C#`
 - 개발 팀원
    - **1인 개발**
 - 소개 : 모바일 기기 해상도 대응 및 최적화 연구를 위해 진행한 1인 프로젝트입니다.

## 주요 기능
 1. 어드레서블을 사용하여 원격 다운로드 시스템 구현 및 최적화
    - 어드레서블 매니저를 구현하여 중복 핸들 생성을 방지
    - 그룹 별 패킹설정 및 압축설정을 통해 다운로드 용량 최적화
 2. 씬 별 싱글 엔트리 포인트를 적용
    - 초기화, 이벤트 구독 및 구독 해제 순서를 보장하여 기기마다 달라질 수 있는 초기화 순서 문제를 해결
 3. 다중 해상도 비율 대응
    - 노치, 펀치홀 등의 대응을 위해 safearea를 구현하여 안전영역을 설정
    - UI별 Anchor를 이용하여 기본 위치 대응
    - 각 구역 별 레이아웃 세팅 스크립트를 이용하여 화면 비율에 방해받지 않는 게임 플레이 환경을 조성
 4. 캔버스 분할 및 스프라이트 아틀라스 적용을 이용한 최적화
    - 내부 UI 변경 빈도 및 목적에 따라 캔버스를 분할하여 캔버스 리빌딩 타임을 최적화
    - 아이템 등장 빈도에 따른 아틀라스 별 스프라이트 종류 및 개수를 조절하여 메모리 사용량 및 드로우콜 최적화
 5. 스크립터블 오브젝트를 이용한 데이터 기반 설계 및 세팅 자동화 설정
    - 데이터를 각각의 SO에 작성 및 커스텀 에디터를 구현하여 버튼 클릭 한 번으로 필요한 데이터를 추가 및 불필요한 데이터를 제거 하도록 구현
    - SO는 원격 어드레서블에 등록하여 앱을 업데이트 하지 않고도 어드레서블 빌드를 통해 수치를 조절 할 수 있도록 시스템을 구현
 6. 프로파일러, 프레임 디버거, 메모리 프로파일러를 이용한 최적화
 7. UniTask를 이용한 GC할당 최적화

## 프로젝트 설치 방법
1. [Unity Hub](https://unity.com/kr/download)를 설치한다.
2. `Unity 6000.3.9f1`버전의 에디터를 설치한다.
3. 저장소를 클론한다.
   ```bash
   git clone https://github.com/woorija/MergeGamePublic.git
   ```
4. Unity Hub에서 프로젝트를 실행한다.

## 게임 실행
 1. [다운로드](https://drive.google.com/drive/folders/1d6pMfzByOLl-tOEjSMzBibcnTpHg3yHP)
 2. 기기에서 apk를 이용해 앱을 설치 후 실행

## 게임 조작법
 화면을 터치 및 드래그하여 게임을 진행

## 스크린샷
<img width="205" height="445" alt="Image" src="https://github.com/user-attachments/assets/f9033ca8-1f7a-4a77-be3a-7d34decc6792" />
<img width="205" height="445" alt="Image" src="https://github.com/user-attachments/assets/1f0a35f2-7060-4960-b6e1-eb80404d5023" />
<img width="205" height="445" alt="Image" src="https://github.com/user-attachments/assets/265a076a-2df6-4748-8873-42f43e33330b" />
<img width="205" height="445" alt="Image" src="https://github.com/user-attachments/assets/8837331e-220a-4f6b-86c2-19f50ad4d631" />
<img width="205" height="445" alt="Image" src="https://github.com/user-attachments/assets/ddd8de0c-153b-4afb-b17b-9354e3de3764" />
<img width="205" height="445" alt="Image" src="https://github.com/user-attachments/assets/bea235d5-b2a2-4d39-be9a-d7bccdc122f0" />

## 플레이 영상
[![title](https://github.com/user-attachments/assets/f9033ca8-1f7a-4a77-be3a-7d34decc6792)](https://youtu.be/Gr1L5oSgdCw)

## 라이센스
```
This project uses the following third-party libraries:

UniTask
Copyright (c) 2019 Yoshifumi Kawai / Cysharp, Inc.
Licensed under the MIT License.
https://github.com/Cysharp/UniTask

KCC 간판체
한국저작권위원회, https://www.copyright.or.kr/main.do
```
