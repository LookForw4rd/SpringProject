using System.Collections;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class CollectItem : MonoBehaviour
{
    public int itemIndex; // 当前收集物在ui左上角收集物槽中的index
    public Sprite itemIcon; // 收集物作为ui的icon
    public Canvas canvas; // 当前收集物ui所处Canvas
    public float flyDuration; // 收集物ui飘到收集物槽中的时间间隔
    public float minCurveOffset = 100f; // 飞行轨迹偏离直线的最小幅度
    public float maxCurveOffset = 300f; // 飞行轨迹偏离直线的最大幅度

    // 当玩家接触收集物item的时候触发其飞向slot的相关事件
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.GetComponent<ElfController>() != null) {
            RectTransform targetSlot = CollectionManager.instance.GetSlotTransform(itemIndex);
            if (targetSlot != null)
                StartCoroutine(FlyToSlot(targetSlot));
        } 
    }

    private IEnumerator FlyToSlot(RectTransform targetSlot) {
        SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = false;
        
        // 创建一个在canvas中的收集物ui game object
        GameObject flyItem = new GameObject("flyCollectItem"); 
        flyItem.transform.SetParent(canvas.transform, false);
        Image image = flyItem.AddComponent<Image>(); // 在ui game object上增加image component
        image.sprite = itemIcon;

        // 将作为物品的收集物的位置获取后传递给ui game object
        Vector2 startPos; 
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Camera.main.WorldToScreenPoint(transform.position),
            canvas.worldCamera,
            out startPos);
        image.rectTransform.anchoredPosition = startPos;
        
        Vector2 targetPos;
        Camera uiCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(uiCamera, targetSlot.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPos,
            uiCamera,
            out targetPos);
        
        // 计算控制点坐标（飞行轨迹设定为一条贝塞尔曲线）
        Vector2 midPoint = (startPos + targetPos) / 2f;
        Vector2 direction = (targetPos - startPos).normalized;
        Vector2 perpendicular = new Vector2(-direction.y, direction.x);
        float randomOffset = Random.Range(minCurveOffset, maxCurveOffset); // 随机获取偏移量
        if (Random.value > 0.5f)    
            randomOffset = -randomOffset; // 随机获取凸起方向
        Vector2 controlPoint = midPoint + perpendicular * randomOffset;

        float elapsed = 0;
        while (elapsed < flyDuration) {
            elapsed += Time.deltaTime;
            float t = elapsed / flyDuration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t); // 缓入缓出效果
            Vector2 currentPos = Mathf.Pow(1 - smoothT, 2) * startPos + // 二次贝塞尔曲线公式
                                 2 * smoothT * (1 - smoothT) * controlPoint + 
                                 Mathf.Pow(smoothT, 2) * targetPos;
            image.rectTransform.anchoredPosition = currentPos;
            yield return null;
        }
        image.rectTransform.anchoredPosition = targetPos;

        // 将对应收集物品槽的image改为收集物的sprite
        targetSlot.GetComponent<Image>().sprite = itemIcon;
        targetSlot.GetComponent<Image>().color = Color.white;
        Destroy(gameObject);
        Destroy(flyItem);
    }
}
