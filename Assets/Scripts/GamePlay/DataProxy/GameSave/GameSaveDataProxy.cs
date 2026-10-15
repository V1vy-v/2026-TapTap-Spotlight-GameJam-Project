using Framework;
using GamePlay.GameSave;
using UnityEngine;

namespace GamePlay.DataProxy
{
    /// <summary>
    /// 本机存档数据代理，加入远端时不使用
    /// </summary>
    public class GameSaveDataProxy : BaseDataProxy
    {
        public override string DataPath => "SaveData";  // 存储到本地的文件名，默认保存在Application.persistentDataPath下
        public override bool IsNeedSaveToLocal => true; // 需要存储到本地，设置为true后就会执行Load/Save函数

        private GameSaveData _gameSave;

        /// <summary>
        /// 更新玩家最后的位置
        /// </summary>
        public void UpdatePlayerPos(Vector3 pos)
        {
            _gameSave.PlayerPos = pos;
            
            // 标记数据为脏，这样就会自动处理保存
            MarkDataDirty();
        }

        #region 属性
        public Vector3 PlayerPos => _gameSave.PlayerPos;
        #endregion

        public override void Load()
        {
            if (Global.LoadJson<GameSaveData>(DataPath, out var data))
            {
                _gameSave = data;
                return;
            }

            _gameSave = new GameSaveData();
            MarkDataDirty();
        }

        public override void Save()
        {
            Global.SaveJson(DataPath, _gameSave);
        }

        public override void Destroy()
        {
            _gameSave = null;
        }
    }
}
