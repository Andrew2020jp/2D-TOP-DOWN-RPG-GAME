using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class resetfunction : MonoBehaviour
{
    public GameObject player;                   // 玩家对象
    public string spawnPointName = "SpawnPoint";
    public CinemachineVirtualCamera vcam;       // 摄像机

    public void Restart()
    {
        // 重置 Flag
        if (FlagHolder.Instance != null)
        {
            FlagHolder.Instance.isSpecialEnemyDefeated = false;
            Debug.Log("Flag has been lowered. The path will be blocked again.");
        }

        if (EconomyManager.Instance != null)
        {
            EconomyManager.Instance.ResetGold();
            Debug.Log("Gold has been reset.");
        }

        if (Stamina.Instance != null)
        {
            Stamina.Instance.ResetStamina();
            Debug.Log("Stamina has been reset.");
        }

        // 重新加载 Scene1
        SceneManager.LoadScene("Scene1");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (EconomyManager.Instance != null)
        {
            EconomyManager.Instance.UpdateGoldUI();
            Debug.Log("Gold UI has been updated.");
        }

        if (Stamina.Instance != null)
        {
            Stamina.Instance.UpdateStaminaImages();
            Debug.Log("Stamina UI has been updated.");
        }

        // 找到玩家对象
        if (player == null)
            player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            // 通过反射修改 PlayerHealth private 字段
            var healthComp = player.GetComponent<PlayerHealth>();
            if (healthComp != null)
            {
                var type = typeof(PlayerHealth);
                var currentField = type.GetField("currentHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var maxField = type.GetField("maxHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                if (currentField != null && maxField != null)
                {
                    int maxHealth = (int)maxField.GetValue(healthComp);
                    currentField.SetValue(healthComp, maxHealth);

                    // 调用 UpdateHealthSlider 刷新血条
                    var updateMethod = type.GetMethod("UpdateHealthSlider", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (updateMethod != null)
                        updateMethod.Invoke(healthComp, null);
                }
            }
        }

        // 重置玩家位置
        GameObject sp = GameObject.Find(spawnPointName);
        if (sp != null && player != null)
            player.transform.position = sp.transform.position;

        // 重新绑定摄像机
        if (vcam == null)
            vcam = FindObjectOfType<CinemachineVirtualCamera>();
        if (vcam != null && player != null)
            vcam.Follow = player.transform;

        // 取消订阅事件
        SceneManager.sceneLoaded -= OnSceneLoaded;
        PlayerHealth.Instance.TOWN_TEXT = "Scene1";
        Debug.Log("Scene1 reloaded and player reset.");
    }
}
