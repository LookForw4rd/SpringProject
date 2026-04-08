using UnityEngine;
using UnityEngine.UI;

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager instance;
    public Image progressBarFill; // 填充进度条的image引用
    public float maxProgress = 100f; // 进度条设置最大进度值
    private float currentProgress = 0f; // 当前游戏已经累积的进度值

    private void Awake() {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start() {
        UpdateUI();
    }

    // 向全局暴露的增加进度值的方法
    public void AddProgress(float amount) {
        currentProgress += amount;
        currentProgress = Mathf.Clamp(currentProgress, 0, maxProgress); // 限制当前的进度值在0和最大值之间
        UpdateUI();
    }

    // 更新进度条内部填充图片的fillAmount属性
    private void UpdateUI() {
        progressBarFill.fillAmount = currentProgress / maxProgress;
    }
}
