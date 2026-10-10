using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DiaLogController : MonoBehaviour
{
    /// <summary>单例</summary>
    public static DiaLogController Instance;

    /// <summary>对话文件名（不带后缀，路径相对 Resources）</summary>
    public string dialogFileName = "";

    [Header("角色数据库")]
    /// <summary>角色 SO 资产，Inspector 里拖</summary>
    public CharacterDatabase characterDatabase;

    private Image spriteLeft;    //角色A立绘
    private Image spriteRight;   //角色B立绘
    private TMP_Text nameText;   //角色名字text
    private TMP_Text dialogText; //文本内容text
    private Button next;         //下一步按钮
    private GameObject optionButton;//选项按钮
    private Transform buttonGroup;   //选项按钮父物体，用于排版

    /// <summary>当前对话索引</summary>
    private int dialogIndex;

    /// <summary>对话文本按行分割</summary>
    private string[] dialogRows;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // 检查 SO 配置
        if (characterDatabase == null)
            Debug.LogError("[DiaLogmanager] 没有配置 CharacterDatabase！");

        // 加载对话文件
        if (!string.IsNullOrEmpty(dialogFileName))
            LoadDialogFile();
    }

    private void Start()
    {
        if (dialogRows != null && dialogRows.Length > 0)
            ShowDiaLogRow();
    }
    /// <summary>
    /// 由 DialogPanel 注入控件引用
    /// </summary>
    public void SetupUI(
        Image left, Image right,
        TMP_Text name, TMP_Text dialog,
        Button nextBtn,
        GameObject optionBtn = null, Transform btnGroup = null)
    {
        spriteLeft = left;
        spriteRight = right;
        nameText = name;
        dialogText = dialog;
        next = nextBtn;
        optionButton = optionBtn;
        buttonGroup = btnGroup;
    }
    /// <summary>
    /// 加载文本文件
    /// </summary>
    private void LoadDialogFile()
    {
        TextAsset dialogDataFile = Resources.Load<TextAsset>(dialogFileName);

        if (dialogDataFile == null)
        {
            Debug.LogError($"找不到对话文件：Resources/{dialogFileName}.csv");
            return;
        }

        ReadText(dialogDataFile);
    }

    public void ReadText(TextAsset _textAsset)
    {
        dialogRows = _textAsset.text.Split('\n');
        Debug.Log("读取成功");
    }

    public void ReadTextFromString(string content)
    {
        dialogRows = content.Split('\n');
        Debug.Log("从字符串读取成功");
    }
    /// <summary>
    /// 设置文本文件接口，需要放在Resources下
    /// </summary>
    /// <param name="fileName"></param>
    public void SetDialogFile(string fileName)
    {
        dialogFileName = fileName;
        dialogIndex = 0;
        ClearOptionButtons();
        LoadDialogFile();
        ShowDiaLogRow();
    }
    /// <summary>
    /// 临时测试接口，不推荐使用
    /// </summary>
    /// <param name="content"></param>
    public void SetDialogContent(string content)
    {
        dialogIndex = 0;
        ClearOptionButtons();
        ReadTextFromString(content);
        ShowDiaLogRow();
    }

    public void UpdateText(string _name, string _text)
    {
        if (nameText != null) nameText.text = _name;
        if (dialogText != null) dialogText.text = _text;
    }

    public void UpdateImage(string _name, string _position)
    {
        spriteLeft.gameObject.SetActive(false);
        spriteRight.gameObject.SetActive(false);

        if (characterDatabase == null) return;
        var icon = characterDatabase.GetIcon(_name);
        if (icon == null) return;

        if (_position == "左")
        {
            spriteLeft.sprite = icon;
            spriteLeft.gameObject.SetActive(true);
        }
        else if (_position == "右")
        {
            spriteRight.sprite = icon;
            spriteRight.gameObject.SetActive(true);
        }
    }
    /// <summary>
    /// 根据 dialogIndex 显示当前对话行
    /// </summary>
    public void ShowDiaLogRow()
    {
        if (dialogRows == null || dialogRows.Length == 0) return;

        for (int i = 0; i < dialogRows.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(dialogRows[i])) continue;

            string[] cells = dialogRows[i].Split(',');
            if (cells.Length < 2) continue;

            // 普通对话
            if (cells[0] == "#" && int.Parse(cells[1]) == dialogIndex)
            {
                UpdateText(cells[2], cells[4]);
                UpdateImage(cells[2], cells[3]);
                dialogIndex = int.Parse(cells[5]);
                next.gameObject.SetActive(true);
                break;
            }
            // 选项
            else if (cells[0] == "&" && int.Parse(cells[1]) == dialogIndex)
            {
                next.gameObject.SetActive(false);
                GenerateOption(i);
            }
            // 结束
            else if (cells[0] == "end" && int.Parse(cells[1]) == dialogIndex)
            {
                // 对话结束的处理逻辑
                break;
            }
        }
    }

    public void OnClickNext()
    {
        ShowDiaLogRow();
    }
    /// <summary>
    /// 选项生成
    /// </summary>
    /// <param name="_index"></param>

    public void GenerateOption(int _index)
    {
        string[] cells = dialogRows[_index].Split(',');

        if (cells[0] == "&")
        {
            GameObject button = Instantiate(optionButton, buttonGroup);
            button.GetComponentInChildren<TMP_Text>().text = cells[4];
            button.GetComponent<Button>().onClick.AddListener(delegate
            {
                OnOptionClick(int.Parse(cells[5]));
            });

            GenerateOption(_index + 1);
        }
    }
    /// <summary>
    /// 选项点击
    /// </summary>
    /// <param name="_id"></param>
    public void OnOptionClick(int _id)
    {
        dialogIndex = _id;
        ShowDiaLogRow();
        ClearOptionButtons();
    }
    /// <summary>
    /// 清楚选项
    /// </summary>
    private void ClearOptionButtons()
    {
        if (buttonGroup == null) return;
        for (int i = buttonGroup.childCount - 1; i >= 0; i--)
        {
            Destroy(buttonGroup.GetChild(i).gameObject);
        }
    }
}