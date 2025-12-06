using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeManager : MonoBehaviour
{
    public static SkillTreeManager Instance;    // �떛湲��넠 �씤�뒪�꽩�뒪

    [Header("Data")]
    public SkillTreeSaveData saveData;
    public PlayerLevelSaveData levelData;

    [Header("Line")]
    public GameObject linePrefab;   // �끂�뱶 �뿰寃고븷 �씪�씤 �봽由ы뙶 蹂��닔
    public Color lockedLineColor = new Color(0.3f, 0.3f, 0.3f, 1f);
    public Color unlockedLineColor = Color.white;

    [Header("臾닿린蹂� LineParent")]
    public Transform lineParentW1;
    public Transform lineParentW2;
    public Transform lineParentW3;

    [Header("SkillTree Tab Controller")]
    public SkillTreeTabController tabController;

    [Header("Current Level")]
    public Text currentLevelText;

    // 臾닿린 媛뺥솕 �뒪�꺈
    private class DamageBonus
    {
        public float flatAddTotal = 0f;     // 怨좎젙媛� �빀怨�
        public float multiplierTotal = 1f;  // 諛곗쑉 怨깃퀎
    }

    private Dictionary<NodeData, Node> nodeLookup = new Dictionary<NodeData, Node>();
    private Dictionary<WeaponData, DamageBonus> weaponBonusMap = new Dictionary<WeaponData, DamageBonus>();

    public void Awake()
    {
        // �떛湲��넠
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // �뵮 諛붽뺨�룄 �뙆愿댄븯吏� �븡湲�
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // [�뀒�뒪�듃�슜] 寃뚯엫 �떆�옉�븷 �븣留덈떎 援щℓ 紐⑸줉 �떦 鍮꾩슦湲�
        // �뀒�뒪�듃 �걹�굹硫� �씠 以꾩�� 瑗� 吏��슦嫄곕굹 二쇱꽍 泥섎━!!!!!!!!!!
        if (saveData != null)
        {
            saveData.purchasedNodes.Clear();
            Debug.Log("�뀒�뒪�듃瑜� �쐞�빐 援щℓ 紐⑸줉�쓣 珥덇린�솕�뻽�뒿�땲�떎.");
        }
    }

    // ����옣�맂 �뜲�씠�꽣 遺덈윭����꽌 �솕硫댁뿉 �떎�떆 洹몃━湲�
    public void Start()
    {
        // �씪�씤 �깮�꽦 �쟾�뿉 紐⑤뱺 �꺆 酉곕�� �솢�꽦�솕�빐�꽌 UI 怨꾩궛�씠 �젣���濡� �릺寃� �븿
        if (tabController != null)
        {
            tabController.ActivateAllViewsTemporarily();
        }
        else
        {
            Debug.LogError("Tab Controller媛� �뿰寃곕릺吏� �븡�븯�뒿�땲�떎.");
        }

        // �씪�씤 �깮�꽦
        GenerateAllLines();

        // �씪�씤 �깮�꽦�씠 �걹�궗�쑝�땲 �꺆 �긽�깭瑜� 珥덇린�솕 (1踰덈쭔 耳쒓퀬 �굹癒몄�� �걫)
        if (tabController != null)
        {
            tabController.InitializeTabs();
        }

        // 2. ����옣�맂 �끂�뱶 �뜲�씠�꽣 遺덈윭����꽌 援щℓ�븳 �끂�뱶 �깋 蹂�寃�
        RefreshUI();

        currentLevelText.text = $"Current Level : {levelData.level}";
    }

    // 援щℓ 媛��뒫�븳 �끂�뱶�씤吏� �젅踰� 泥댄겕
    public bool IsLevelAllowed(NodeData data)
    {
        // 1�젅踰⑤쭏�떎 �끂�뱶 2媛쒖뵫 援щℓ 媛��뒫
        int maxOrder = levelData.level * 1;
        return data.orderInBranch <= maxOrder;
    }

    // 援щℓ 媛��뒫�븳 �끂�뱶�씤吏� �솗�씤(留덉슦�뒪 �겢由��뻽�쓣 �븣)
    public bool TryPurchase(NodeData data)
    {
        // 0. �꽑�뻾 �끂�뱶媛� 援щℓ �븞�릱�쓬
        if (data.preNode != null && !saveData.purchasedNodes.Contains(data.preNode))
        {
            Debug.Log($"�쁽�옱 �젅踰�: {levelData.level}\n>> �끂�뱶 援щℓ �떎�뙣: �꽑�뻾 �끂�뱶瑜� 癒쇱�� 援щℓ�빐�빞 �빀�땲�떎.");
            return false;
        }
        // 1. �젅踰� �젣�븳
        if (!IsLevelAllowed(data))
        {
            Debug.Log($"�쁽�옱 �젅踰�: {levelData.level}\n>> �끂�뱶 援щℓ �떎�뙣: �젅踰⑥씠 遺�議깊빀�땲�떎.");
            return false;
        }
        // 2. �씠誘� 援щℓ�븳 �끂�뱶�씪 �븣
        if (saveData.purchasedNodes.Contains(data))
        {
            Debug.Log($"�쁽�옱 �젅踰�: {levelData.level}\n>> �씠誘� 援щℓ�븳 �끂�뱶�엯�땲�떎.");
            return false;
        }
        

        // �끂�뱶 援щℓ 泥섎━
        saveData.purchasedNodes.Add(data);
        Debug.Log($">> {data.name} 援щℓ �꽦怨�!");

        // �깋�긽 蹂�寃�
        RefreshUI();

        // �끂�뱶 �겢由��븯硫� �뒪�꺈 利앷��
        ApplyNodeEffect(data);

        return true;
    }

    // 援щℓ�븳 �끂�뱶�씤吏� �솗�씤
    public bool IsPurchased(NodeData data)
    {
        return saveData.purchasedNodes.Contains(data);
    }

    // �끂�뱶 �뿰寃고븯�뒗 �씪�씤 �깮�꽦
    void GenerateAllLines()
    {
        // nodeLookup�뿉 �벑濡앸맂 紐⑤뱺 �끂�뱶瑜� �븯�굹�뵫 爰쇰궡湲�
        foreach (KeyValuePair<NodeData, Node> entry in nodeLookup)
        {
            NodeData data = entry.Key;
            Node currentNode = entry.Value;

            // �꽑�뻾 �끂�뱶媛� �뾾�쑝硫�(猷⑦듃 �끂�뱶硫�) �꽑 �깮�꽦X
            if (data.preNode == null) continue;

            // �꽑 �깮�꽦 �븿�닔 �샇異�
            CreateLineObject(currentNode, data);
        }
    }

    // UI �깉濡쒓퀬移� (�깋源� �뾽�뜲�씠�듃)
    public void RefreshUI()
    {
        // 紐⑤뱺 �끂�뱶瑜� �븯�굹�뵫 寃��궗
        foreach (KeyValuePair<NodeData, Node> entry in nodeLookup)
        {
            NodeData data = entry.Key;
            Node node = entry.Value;

            // 1. �끂�뱶 �깋�긽 泥섎━ (援щℓ�릱�쑝硫� 諛앷쾶, �븘�땲硫� �뼱�몼寃�)
            if (IsPurchased(data))
            {
                node.isPurchased = true;
                node.nodeImage.color = node.hoverColor;
            }
            else
            {
                node.isPurchased = false;
                node.nodeImage.color = node.normalColor;
            }

            // 2. �씪�씤 �깋�긽 泥섎━
            // connectedLine : 遺�紐� �끂�뱶 -> �쁽�옱 �끂�뱶 �뿰寃고븯�뒗 �씪�씤
            if (node.connectedLine != null && data.preNode != null)
            {
                if (IsPurchased(data.preNode))
                {
                    // 遺�紐� �끂�뱶媛� 援щℓ�릺硫� connectedLine 諛앷쾶 蹂�寃�
                    node.connectedLine.color = unlockedLineColor;
                }
                else
                {
                    node.connectedLine.color = lockedLineColor;
                }
            }
        }
    }

    // �씪�씤 �삤釉뚯젥�듃 �떎�젣濡� �솕硫댁뿉 �깮�꽦
    void CreateLineObject(Node childNode, NodeData data)
    {
        Node parentNode = FindNodeInstance(data.preNode);
        if (parentNode == null) return;

        // 遺�紐� LineParent 李얘린
        Transform targetLineParent = null;

        switch (data.branchType) // NodeData�쓽 BranchType
        {
            case BranchType.Weapon1:
            case BranchType.Weapon11:
            case BranchType.Weapon12:
            case BranchType.Weapon13:
                targetLineParent = lineParentW1;
                break;
            case BranchType.Weapon2:
            case BranchType.Weapon21:
            case BranchType.Weapon22:
            case BranchType.Weapon23:
                targetLineParent = lineParentW2;
                break;
            case BranchType.Weapon3:
            case BranchType.Weapon31:
            case BranchType.Weapon32:
            case BranchType.Weapon33:
                targetLineParent = lineParentW3;
                break;
            default:
                Debug.LogError($"[SkillTreeManager] �븣 �닔 �뾾�뒗 BranchType�엯�땲�떎: {data.branchType}, �끂�뱶 �씠由�: {data.name}");
                return; // 遺�紐⑤�� 紐� 李얠쑝硫� �씪�씤 �깮�꽦 以묐떒
        }

        // �씪�씤 �삤釉뚯젥�듃 �깮�꽦
        GameObject lineObj = Instantiate(linePrefab, targetLineParent);
        RectTransform lineRect = lineObj.GetComponent<RectTransform>();
        Image lineImage = lineObj.GetComponent<Image>();

        // �쐞移�, �쉶�쟾
        Vector2 startPosLocal = targetLineParent.InverseTransformPoint(parentNode.GetComponent<RectTransform>().position);
        Vector2 endPosLocal = targetLineParent.InverseTransformPoint(childNode.GetComponent<RectTransform>().position);

        lineRect.pivot = new Vector2(0.5f, 0f);
        lineRect.anchorMin = new Vector2(0.5f, 0.5f);
        lineRect.anchorMax = new Vector2(0.5f, 0.5f);
        lineRect.anchoredPosition = startPosLocal;

        Vector2 dir = endPosLocal - startPosLocal;
        float distance = dir.magnitude;

        // 湲몄씠��� �쉶�쟾
        lineRect.sizeDelta = new Vector2(lineRect.sizeDelta.x, distance);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        lineRect.localRotation = Quaternion.Euler(0, 0, angle);

        lineRect.SetAsLastSibling(); // 留� �븵�뿉 洹몃━湲�
        if (lineImage != null) lineImage.raycastTarget = false; // �젅�씠罹먯뒪�듃 �걚湲�

        // 泥섏쓬�뿏 �뼱�몢�슫 �깋
        lineImage.color = lockedLineColor;

        // �옄�떇 �끂�뱶�뿉 �깮�꽦�맂 �씪�씤 �씠誘몄�� ����옣
        childNode.connectedLine = lineImage;
    }

    // �끂�뱶 �뜲�씠�꽣 �벑濡�
    public void RegisterNode(NodeData data, Node node)
    {
        if (!nodeLookup.ContainsKey(data))
            nodeLookup.Add(data, node);
    }

    // NodeData�뿉 �빐�떦�븯�뒗 �끂�뱶 �삤釉뚯젥�듃 諛섑솚
    public Node FindNodeInstance(NodeData data)
    {
        if (nodeLookup.ContainsKey(data))
            return nodeLookup[data];
        return null;
    }

    // �끂�뱶 援щℓ�븯硫� �뒪�꺈 利앷��
    void ApplyNodeEffect(NodeData data)
    {
        // �슚怨쇨�� �뾾嫄곕굹 ����긽 臾닿린媛� �뾾�쑝硫� �뙣�뒪
        if (data.bonusType == AttackBonusType.None || data.bonusValue == 0f || data.targetWeapons == null) return;

        // �쟻�슜 ����긽 臾닿린 �씠由꾨뱾 �닔吏�(湲덇컯�졊 + �꽍�옣)
        string weaponNamesStr = "";
        for (int i = 0; i < data.targetWeapons.Length; i++)
        {
            if (data.targetWeapons[i] == null) continue;
            weaponNamesStr += data.targetWeapons[i].weaponName;
            if (i < data.targetWeapons.Length - 1) weaponNamesStr += " + ";
        }

        // 媛� 臾닿린蹂꾨줈 蹂대꼫�뒪 �쟻�슜
        foreach (WeaponData weapon in data.targetWeapons)
        {
            if (weapon == null) continue;

            // 1. �씠 臾닿린�쓽 蹂대꼫�뒪 湲곕줉�씠 �뾾�쑝硫� �깉濡� 留뚮벀
            if (!weaponBonusMap.ContainsKey(weapon))
            {
                weaponBonusMap.Add(weapon, new DamageBonus());
            }

            // 2. 蹂대꼫�뒪 ����엯�뿉 �뵲�씪 媛믪쓣 �늻�쟻 湲곕줉
            DamageBonus currentBonus = weaponBonusMap[weapon];

            // 理쒖쥌 怨듦꺽�젰 怨꾩궛�쓣 �쐞�빐 湲곕낯 怨듦꺽�젰 媛��졇�삤湲�
            float baseDamage = weapon.damage;
            float currentFinalDamage = 0f;

            switch (data.bonusType)
            {
                case AttackBonusType.FlatAdd:
                    currentBonus.flatAddTotal += data.bonusValue;

                    // �쁽�옱 �떆�젏�쓽 理쒖쥌 怨듦꺽�젰 怨꾩궛: (湲곕낯 + 怨좎젙�빀) * 諛곗쑉怨�
                    currentFinalDamage = (baseDamage + currentBonus.flatAddTotal) * currentBonus.multiplierTotal;

                    // �쁽�옱 理쒖쥌 怨듦꺽�젰 異쒕젰
                    Debug.Log($" [{weaponNamesStr}] '{weapon.weaponName}' 怨듦꺽�젰 +{data.bonusValue} (�쁽�옱 '{weapon.weaponName}' 怨듦꺽�젰: {currentFinalDamage:F2})");
                    break;

                case AttackBonusType.Multiplier:
                    currentBonus.multiplierTotal *= data.bonusValue;

                    // �쁽�옱 �떆�젏�쓽 理쒖쥌 怨듦꺽�젰 怨꾩궛
                    currentFinalDamage = (baseDamage + currentBonus.flatAddTotal) * currentBonus.multiplierTotal;

                    // �쁽�옱 理쒖쥌 怨듦꺽�젰 異쒕젰 (�냼�닔�젏 2�옄由ш퉴吏� �몴�떆)
                    Debug.Log($" [{weaponNamesStr}] '{weapon.weaponName}' 怨듦꺽�젰 x{data.bonusValue} (�쁽�옱 '{weapon.weaponName}' 怨듦꺽�젰: {currentFinalDamage:F2})");
                    break;
            }
        }
    }

    // �쇅遺��뿉�꽌 理쒖쥌 �뜲誘몄��瑜� �슂泥��븷 �븣 怨꾩궛�븯�뒗 �븿�닔
    public float GetFinalWeaponDamage(WeaponData weapon)
    {
        float baseDamage = weapon.damage; // �썝蹂� 怨듦꺽�젰

        // 蹂대꼫�뒪 湲곕줉�씠 �뾾�쑝硫� �썝蹂� 洹몃��濡� 諛섑솚
        if (!weaponBonusMap.ContainsKey(weapon))
        {
            return baseDamage;
        }

        // 湲곕줉�씠 �엳�쑝硫� 怨꾩궛 �떆�옉
        DamageBonus bonus = weaponBonusMap[weapon];

        // 理쒖쥌 �뜲誘몄�� = (�썝蹂� + 怨좎젙媛� 珥앺빀) * 諛곗쑉 珥앷낢
        float finalDamage = (baseDamage + bonus.flatAddTotal) * bonus.multiplierTotal;

        return finalDamage;
    }
}
