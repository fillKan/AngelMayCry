# AngelMayCry
> 졸업작품 프로젝트 앓~
>
> Unity Editor Version : 2020.3.0f1
> [**유니티 버전 아카이브**](https://unity3d.com/kr/get-unity/download/archive)

# 네이밍 룰

## 1. 모든 이름은 대문자로 시작
변수, 함수, 클래스, 구조체, 열거형, 열거자 등등...이름을 대문자로 시작한다
```c#

public string ApplePen;
public string PineApplePen;

private void Uh() {...}

private IEnumerator PenPineAppleApplePen() {...}

```
하지만? 지역변수와 매개변수는 예외로, 이름이 **소문자**로 시작한다
```c#
private string Sans(bool youKnow) 
{
  string dancingSans = youKnow ? "와!" : "와!";
  return dancingSane;
}
```

## 2. public이 아닌 멤버변수는 이름 앞에 _를 붙인다
private와 protected로 지정된 멤버변수는 이름 앞에 `_`를 붙인다
```c#
public class LookSoCool
{
  public int Sixteen = 16;
  protected int _Sixteen = 16;
}
```
