using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class GachaSystem : MonoBehaviour
{
    [SerializeField] private GachaText _gachaText;
    [SerializeField] private Transform _gachaParent;
    // アイテム名とその排出重みを定義する構造体
    [System.Serializable]
    public struct GachaItem
    {
        public string ItemName;
        [Tooltip("排出される重み。合計値が100である必要はないが、相対的な確率となる。")]
        public int Weight;
    }

    // 抽選対象のアイテムリスト
    public List<GachaItem> Items = new List<GachaItem>();

    private void Start()
    {
        // サンプルとして抽選を10回実行
        for (int i = 0; i < 10; i++)
        {
            var text = Instantiate(_gachaText, _gachaParent);
            text.Initialize(DrawItem());
        }
    }

    /// <summary>
    /// 重み付き確率でアイテムを抽選するメソッド
    /// </summary>
    /// <returns>抽選で選ばれたアイテムの名前</returns>
    public GachaItem DrawItem()
    {
        // 1. 全アイテムの重みの合計を計算する
        // LinqのSum()を使ってリスト内の全weightを合計
        int totalWeight = Items.Sum(item => item.Weight);

        if (totalWeight <= 0)
        {
            Debug.LogError("抽選アイテムの重みの合計が0以下です。アイテムリストを確認してください。");
            return default;
        }

        // 2. 0から合計重みまでの間の乱数を生成する
        // UnityEngine.Random.Range()の第二引数は排他的なので、+1は不要
        int randomPoint = Random.Range(0, totalWeight);

        // 3. 乱数がどのアイテムの重みの範囲に落ちたかを判定する
        int currentWeightSum = 0;
        foreach (var item in Items)
        {
            // 現在のアイテムの重みを加算し、境界値を更新
            currentWeightSum += item.Weight;

            // 生成した乱数が、このアイテムの重みの範囲内にあるか判定
            if (randomPoint < currentWeightSum)
            {
                // 抽選成功。このアイテムを返す
                return item;
            }
        }

        // 理論上ここには到達しないが、フォールバック
        return default;
    }
}