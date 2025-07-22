
public class FinalArcherArrow : ArcherArrow
{
    // 최종 궁수 화살은 기본 궁수 화살과 동일한 기능을 수행하지만,
    // Hit 메서드에서 추가적인 로직을 구현할 수 있습니다.
    protected override void Hit(Monster target)
    {
        base.Hit(target);
        // 추가적인 로직을 여기에 작성할 수 있습니다.
    }
}