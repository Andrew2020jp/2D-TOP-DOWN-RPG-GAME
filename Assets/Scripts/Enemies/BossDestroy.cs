using UnityEngine;
using System.Collections; // 引入命名空间以使用协程

public class BossDestroy : MonoBehaviour
{
    // 使用SerializeField将gameObject序列化并显示在Inspector中
    [SerializeField]
    private GameObject targetGameObject; // 目标GameObject

    private void Start()
    {
        // 启动协程，1秒后将目标GameObject设置为不激活
        StartCoroutine(DeactivateGameObjectAfterDelay(0.1f));
    }

    // 协程：在指定时间后将目标GameObject设置为不激活
    private IEnumerator DeactivateGameObjectAfterDelay(float delay)
    {
        // 等待指定的延迟时间
        yield return new WaitForSeconds(delay);

        // 确保目标GameObject初始为不激活状态
        if (targetGameObject != null)
        {
            targetGameObject.SetActive(false);
        }
    }

    // 当Boss被销毁时会调用这个函数
    private void OnDestroy()
    {
        // Boss被销毁时激活目标GameObject
        if (targetGameObject != null)
        {
            targetGameObject.SetActive(true);
        }
    }
}
