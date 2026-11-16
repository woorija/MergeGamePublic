using System.Collections.Generic;

public class OrderRuleLevelDataBase : SingletonBehaviour<OrderRuleLevelDataBase>
{
    OrderRuleLevelTableSO tableSO;

    public void Init(OrderRuleLevelTableSO _tableSO)
    {
        tableSO = _tableSO;
    }
    public int GetRuleLevel(int playerLevel)
    {
        List<int> levels = tableSO.levels;

        int result = levels[0];

        for (int i = 0; i < levels.Count; i++)
        {
            if (levels[i] > playerLevel)
            {
                break;
            }

            result = levels[i];
        }

        return result;
    }
}
