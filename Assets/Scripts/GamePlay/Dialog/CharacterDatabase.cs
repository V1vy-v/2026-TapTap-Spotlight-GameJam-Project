using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 单个角色数据条目
/// </summary>
[System.Serializable]
public class CharacterEntry
{
    public string name;     // 角色名字
    public Sprite icon;     // 角色立绘/头像
}

/// <summary>
/// 角色数据库：所有对话系统共享一份
/// </summary>
[CreateAssetMenu(fileName = "CharacterDatabase", menuName = "Dialog/角色数据库")]
public class CharacterDatabase : ScriptableObject
{
    [SerializeField]
    private List<CharacterEntry> characters = new List<CharacterEntry>();

    private Dictionary<string, Sprite> _cache;

    /// <summary>
    /// 根据名字获取图标
    /// </summary>
    public Sprite GetIcon(string name)
    {
        BuildCacheIfNeeded();

        if (_cache.TryGetValue(name, out var icon))
            return icon;

        Debug.LogWarning($"[CharacterDatabase] 找不到角色：{name}");
        return null;
    }

    /// <summary>
    /// 是否包含某角色
    /// </summary>
    public bool Contains(string name)
    {
        BuildCacheIfNeeded();
        return _cache.ContainsKey(name);
    }

    /// <summary>
    /// 运行时字典缓存
    /// </summary>
    private void BuildCacheIfNeeded()
    {
        if (_cache != null && _cache.Count == characters.Count) return;

        _cache = new Dictionary<string, Sprite>();
        foreach (var c in characters)
        {
            if (string.IsNullOrEmpty(c.name) || c.icon == null) continue;
            _cache[c.name] = c.icon;
        }
    }

    // 编辑时改了列表，清掉缓存
    private void OnValidate()
    {
        _cache = null;
    }
}