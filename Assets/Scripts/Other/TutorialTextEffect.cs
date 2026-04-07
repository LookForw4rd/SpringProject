using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialTextEffect : MonoBehaviour
{
    public GameObject wordPrefab; // 预制体文本框
    public float screenWidth = 1920f; // 当前屏幕分辨率，确保能够飞出屏幕就
    public float wordSpacing = 20f; // 文本框中间的间距
    public float moveInDuration = 1.5f; // 当前文本飘入屏幕所需时间
    public float stayDuration = 3f; // 当前文本停留屏幕所需时间
    public float moveOutDuration = 1.5f; // 当前文本离开屏幕所需时间
    public float waveHeight = 30f; // 当前文本上下飘动幅度
    public float waveSpeed = 5f; // 当前文本上下飘动的速度
    public float waveOffset = 1f; // 当前文本飘动时带来错落感的相位差
    public float textSpawnDelay = 0.15f; // 当前文本每个单词出现的时间间隔
    public Color buttonTextColor;
    public Color otherTextColor;
    
    public RectTransform container; // 保存字符串的多个文本框prefab的父节点
    private string currentText;

    public void PlayMoveTutorial() {
        currentText = "Use A D to move";
        StartCoroutine(SpawnAndAnimateWords(currentText));
    }

    // 使用此携程讲一个字符串飘入屏幕、停留并飘出
    private IEnumerator SpawnAndAnimateWords(string text) {
        string[] words = text.Split(' ');
        List<RectTransform> wordRects = new List<RectTransform>();
        float[] wordWidths = new float[words.Length];
        float totalWidth = 0f;

        for (int i = 0; i < words.Length; i++) {
            GameObject wordObj = Instantiate(wordPrefab, container);
            TextMeshProUGUI tmp = wordObj.GetComponent<TextMeshProUGUI>();
            tmp.text = words[i];

            if (tmp.text.Length == 1 && tmp.text[0] >= 'A') // 按键对应字母
                tmp.color = buttonTextColor;
            else 
                tmp.color = otherTextColor;
            
            RectTransform rect = wordObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            
            tmp.ForceMeshUpdate();
            wordWidths[i] = tmp.preferredWidth;
            totalWidth += wordWidths[i];
            
            wordRects.Add(wordObj.GetComponent<RectTransform>());
            wordRects[i].anchoredPosition = new Vector2(-2000f, 0); // 初始状态下单词框静置于scene左侧很远的位置
        }
        totalWidth += (words.Length - 1) * wordSpacing;

        float currentX = -totalWidth / 2f;
        float[] targetXPositions = new float[words.Length]; // 计算所有文本框的相对x轴位置
        for (int i = 0; i < words.Length; i++) {
            targetXPositions[i] = currentX + (wordWidths[i] / 2f);
            currentX += wordWidths[i] + wordSpacing;
        }

        for (int i = 0; i < words.Length; i++) { // 具体每个单词的生成和动画化过程
            StartCoroutine(AnimateSingleWord(wordRects[i], targetXPositions[i], i));
            yield return new WaitForSeconds(textSpawnDelay);
        }
    }

    private IEnumerator AnimateSingleWord(RectTransform rect, float targetX, int wordIndex) {
        float startX = -screenWidth;
        float endX = screenWidth;
        float wavePhaseOffset = wordIndex * waveOffset;

        // 单词飘入过程
        float timer = 0f;
        while (timer < moveInDuration) {
            timer += Time.deltaTime;
            float t = timer / moveInDuration;
            float easeT = 1f - Mathf.Pow(1f - t, 3f); // 平滑曲线
            float currentX = Mathf.Lerp(startX, targetX, easeT);
            float currentY = Mathf.Sin(Time.time * waveSpeed - wavePhaseOffset) * waveHeight;
            rect.anchoredPosition = new Vector2(currentX, currentY);
            yield return null;
        }
        
        // 单词停留飘动的过程
        timer = 0f;
        while (timer < stayDuration) {
            timer += Time.deltaTime;
            float currentY = Mathf.Sin(Time.time * waveSpeed - wavePhaseOffset) * ( waveHeight / 2 );
            rect.anchoredPosition = new Vector2(targetX, currentY);
            yield return null;
        }
        
        // 单词飘出过程
        timer = 0f;
        while (timer < moveOutDuration) {
            timer += Time.deltaTime;
            float t = timer / moveOutDuration;
            float easeT = t * t * t; // 平滑加速曲线
            float currentX = Mathf.Lerp(targetX, endX, easeT);
            float currentY = Mathf.Sin(Time.time * waveSpeed - wavePhaseOffset) * waveHeight;
            rect.anchoredPosition = new Vector2(currentX, currentY);
            yield return null;
        }
        
        Destroy(rect.gameObject);
    }
}
