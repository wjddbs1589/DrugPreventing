# 마약예방 VR 체험 콘텐츠 (세종경찰청 외)

스마트폰 DM으로 마약을 접하는 상황부터 투약, 환각, 금단, 돌이킬 수 없는 결과까지를 1인칭으로 체험하는 경찰청 마약예방 교육용 VR 콘텐츠.
세종경찰청 외 여러 기관에 납품했으며, 광주 납품본은 요청에 따라 별도 버전으로 제작했다.

| 항목 | 내용 |
|---|---|
| 기간 | 개발 2개월 + 현장 시연 및 기기 세팅·납품 |
| 인원 | 1인 개발 (기획·에셋 제외) |
| 플랫폼 | Oculus (VR HMD, 스탠드얼론) |
| 기술 | Unity, C#, BNG VR Interaction Framework, URP Post-Processing |

> **안내**
> - 납품 프로젝트의 스크립트 중 체험 흐름과 핵심 연출 코드만 발췌한 저장소로, 단독으로 빌드되지 않는다.
> - 공개를 위해 주석 정리 및 일부 리팩터링을 거친 버전으로, 납품 빌드와 세부 구현이 다를 수 있다.

---

## 체험 흐름

```
시작 메뉴 → 튜토리얼 → 주의사항
→ [미션] 핸드폰 찾기 → 마약 판매 DM 수신
→ 마약 4종 정보 + 부작용 영상 (모두 시청해야 진행)
→ [미션] 필로폰 찾기 → 투약 → 벌레 환각 + 화면 어두워짐
→ 후회 → 금단 속삭임 → [미션] 다른 필로폰 찾기 → 재투약
→ 강아지 장면 → 피 묻은 손 → 강아지 발견 컷신
→ 엔딩 DM + 예방 안내 문구 → 재시작
```

## 폴더 구성

### `Scripts/Flow` — 체험 진행 관리
| 파일 | 역할 |
|---|---|
| `GameManager.cs` | 공통 참조 보관, 미션 안내 UI |
| `CanvasManager.cs` | 시작 UI 흐름, 대사·컷신 중 플레이어 조작 잠금 |
| `DirectionIndicator.cs` | 현재 미션 대상을 가리키는 방향 화살표 |
| `BGMcontroller.cs` | 장면별 배경음 전환 |

### `Scripts/Intro` — 도입
| 파일 | 역할 |
|---|---|
| `SmartPhone.cs` / `SmartPhoneCanvas.cs` | 스마트폰 잡기 → 마약 판매 DM 화면과 대사 |

### `Scripts/DrugInfo` — 약물 정보와 부작용 영상
| 파일 | 역할 |
|---|---|
| `VideoPlayManager.cs` | 약물 4종 선택, 설명창, 부작용 영상 재생(일시정지·2배속·스킵), 시청 완료 관리 |
| `ActiveInfoText.cs` | 약물별 이름·분류·부작용 설명 표시 |
| `LSDeffect.cs` | LSD 체험 연출(URP Volume) 유지 시간 관리 |
| `EndVideo.cs` | 영상 시청 완료 후 필로폰 단계로 전환 |

### `Scripts/Philopon` — 투약·환각·금단 체험
| 파일 | 역할 |
|---|---|
| `Philopon.cs` / `Philopon2.cs` | 첫 번째·두 번째 투약 상호작용 |
| `PhiloponEffect.cs` | Color Adjustments 노출값을 낮춰 화면이 점점 어두워지는 연출 |
| `PhiloponLight.cs` / `BugSpawnerController.cs` | 벌레 환각 연출과 진행 |
| `PhiloponTable2.cs` | 금단 단계, 나레이션 음성 길이에 맞춘 자막 |

### `Scripts/Ending` — 강아지 장면과 엔딩
| 파일 | 역할 |
|---|---|
| `DogSpawn.cs` / `DogDie.cs` | 강아지 등장, 피 묻은 손 머티리얼 교체 |
| `DogEnd.cs` | 강아지 발견 컷신 |
| `TriggerSet.cs` | 강아지 행동 애니메이션 무작위 선택 |
| `Telegram.cs` | 엔딩 DM, 예방 안내 문구, 납품처별 버전 분기 |

### `Scripts/UX` — VR 사용성
| 파일 | 역할 |
|---|---|
| `ObjectVisibility.cs` | 가까워질수록 상호작용 대상 아웃라인 강조 |
| `ParticleAlphaController.cs` | 가까워질수록 파티클 선명하게 표시 |

### `Scripts/Common`
| 파일 | 역할 |
|---|---|
| `TypingEffect.cs` | 대사·자막·미션 안내 공통 타이핑 효과 |

---

## 포함하지 않은 의존 코드
BNG VR Interaction Framework(`PlayerRotation`, `SmoothLocomotion` 등), Oculus Integration(`OVRScreenFade`), `OptionButton`, 아웃라인 컴포넌트(`Outline`), 기타 단순 연출 스크립트 등
