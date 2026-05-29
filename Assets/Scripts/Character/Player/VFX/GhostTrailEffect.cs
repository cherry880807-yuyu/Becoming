using UnityEngine;

public class GhostTrailEffect : MonoBehaviour
{
    [SerializeField] private Camera _ghostCamera;        // 專屬 Ghost Camera
    [SerializeField] private RenderTexture _rt;          // 單張 RT
    [SerializeField] private SpriteRenderer _ghostQuad;  // 顯示 RT 的 SpriteRenderer
    [SerializeField] private float _ghostLifetime = 0.25f;
    [SerializeField] private Color _ghostColor = new Color(0.5f, 0.8f, 1f, 0.7f);

    private static readonly int _colorID = Shader.PropertyToID("_Color");
    private MaterialPropertyBlock _propertyBlock;

    private bool _isActive;
    private float _elapsed;

    private void Awake()
    {
        _propertyBlock = new MaterialPropertyBlock();
        _ghostCamera.enabled = false;
        _ghostQuad.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        EventBus.Subscribe<DodgeSucceededEvent>(SpawnGhostTrail);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<DodgeSucceededEvent>(SpawnGhostTrail);
    }

    public void SpawnGhostTrail(DodgeSucceededEvent e)
    {
        SetGhostCamera();
        SetGhostQuad();
        _elapsed = 0f;
        _isActive = true;
    }
    private void SetGhostCamera()
    {
        //  Ghost Camera 對齊 Player 位置
        _ghostCamera.transform.position = new Vector3(
            transform.position.x,
            transform.position.y,
            _ghostCamera.transform.position.z
        );

        _ghostCamera.targetTexture = _rt;
        _ghostCamera.Render();
        _ghostCamera.targetTexture = null;
    }

    private void SetGhostQuad()
    {
        // Quad 對齊 Player 的 position 和 scale ，並且在Quad 顯示臨時快照畫面 + 染色效果
        _ghostQuad.transform.position = transform.position;
        _ghostQuad.transform.rotation = transform.rotation;

        _ghostQuad.gameObject.SetActive(true);

        _ghostQuad.GetPropertyBlock(_propertyBlock);
        _propertyBlock.SetTexture("_MainTex", _rt);
        _propertyBlock.SetColor(_colorID, _ghostColor);
        _ghostQuad.SetPropertyBlock(_propertyBlock);
    }
    private void Update()
    {
        if (!_isActive) return;

        _elapsed += Time.deltaTime;

        // 淡出
        var c = _ghostColor;
        c.a = Mathf.Lerp(_ghostColor.a, 0f, _elapsed / _ghostLifetime);

        _ghostQuad.GetPropertyBlock(_propertyBlock);
        _propertyBlock.SetColor(_colorID, c);
        _ghostQuad.SetPropertyBlock(_propertyBlock);

        if (_elapsed >= _ghostLifetime)
        {
            _ghostQuad.gameObject.SetActive(false);
            _isActive = false;
        }
    }
}