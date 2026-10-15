/// <summary>
/// buff实例，各自管理其运行状态
/// </summary>
public class BuffInstance
{
    public BuffConfig config;
    public float currentDuration;

    public BuffInstance(BuffConfig config)
    {
        this.config = config;
        this.currentDuration = config.duration;
    }

    
    public bool Update(float deltaTime)
    {
        currentDuration -= deltaTime;
        return currentDuration <= 0;
    }
}