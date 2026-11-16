using UnityEngine;

public class testscript : MonoBehaviour
{
    private void Start()
    {
        DebugFindIndexOrder(1);
    }
    public void Test()
    {
        PlayerDataManager.Instance.Test();
    }
    void DebugFindIndexOrder(int factoryIndex)
    {
        const int COLS = 8;
        const int ROWS = 10;

        int fx = factoryIndex % COLS;
        int fy = factoryIndex / COLS;

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.Append($"[Factory:{factoryIndex}] 생성 순서: ");

        int maxDist = COLS + ROWS;

        for (int dist = 1; dist < maxDist; dist++)
        {
            int seg1 = dist + 1;    // 위쪽:   dx = 0 → +dist,       dy = -dist
            int seg2 = dist * 2;    // 오른쪽: dx = +dist,            dy = -dist+1 → +dist
            int seg3 = dist * 2;    // 아래쪽: dx = +dist-1 → -dist,  dy = +dist
            int seg4 = dist * 2;    // 왼쪽:   dx = -dist,            dy = +dist-1 → -dist
            int seg5 = dist - 1;    // 상단 왼쪽 마무리: dy = -dist, dx = -dist+1 → -1  ← 추가
            int totalInRing = seg1 + seg2 + seg3 + seg4 + seg5; // = dist*8

            for (int i = 0; i < totalInRing; i++)
            {
                int dx, dy;

                if (i < seg1)
                {
                    int j = i;
                    dx = j;
                    dy = -dist;
                }
                else if (i < seg1 + seg2)
                {
                    int j = i - seg1;
                    dx = dist;
                    dy = -dist + 1 + j;
                }
                else if (i < seg1 + seg2 + seg3)
                {
                    int j = i - seg1 - seg2;
                    dx = dist - 1 - j;
                    dy = dist;
                }
                else if (i < seg1 + seg2 + seg3 + seg4)
                {
                    int j = i - seg1 - seg2 - seg3;
                    dx = -dist;
                    dy = dist - 1 - j;
                }
                else
                {
                    // 상단 왼쪽: dy=-dist, dx=-dist+1 → -1
                    int j = i - seg1 - seg2 - seg3 - seg4;
                    dx = -dist + 1 + j;
                    dy = -dist;
                }

                int nx = fx + dx;
                int ny = fy + dy;

                if (nx < 0 || nx >= COLS || ny < 0 || ny >= ROWS)
                    continue;

                int idx = ny * COLS + nx;
                sb.Append($"{idx} ");
            }
        }

        Debug.Log(sb.ToString());
    }
}
