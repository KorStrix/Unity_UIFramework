#region Header
/*	============================================
 *	Author 			    	: Strix
 *	Initial Creation Date 	: 2020-06-15
 *	Summary 		        :
 *
 *  원본 출처 : https://www.youtube.com/watch?v=X-H5Zu3k4Es
 *
 *  기능추가
 *  - 애니메이션 방향 조정
 *  - 업데이트 모드 추가
 *  - 이미지 / 텍스트 범용 적용
 *
 *  Template 		        : New Behaviour For Unity Editor V2
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(Graphic))]
public class UIGradientEffect : BaseMeshEffect
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    public enum EGradientDirection
    {
        Left_To_Right,
        Right_To_Left,
    }

    public enum EUpdateMode
    {
        Time,
        RealTime,
        Manual,
    }

    /* public - Field declaration               */

    public Graphic pGraphicTarget { get; private set; }


    [Header("이펙트 설정")]
    public Gradient pGradient = new Gradient();
    public float fSpeed = 1f;

    [Space(10)]
    [Header("색상변화 방향")]
    public EGradientDirection eGradient_Direction = EGradientDirection.Left_To_Right;

    [Space(10)]
    public EUpdateMode eUpdateMode = EUpdateMode.Time;

    /* protected & private - Field declaration  */

    delegate float OnCalculate_GradientEvaluate(float fMin, float fMax, float fCurrent, float fElapseTime);

    private static readonly Dictionary<EGradientDirection, OnCalculate_GradientEvaluate> g_map_OnCalculateEvaluate = new Dictionary<EGradientDirection, OnCalculate_GradientEvaluate>()
    {
        { EGradientDirection.Left_To_Right, Calculate_Left_To_Right },
        { EGradientDirection.Right_To_Left, Calculate_Right_ToLeft }
    };



    readonly List<UIVertex> _listVertex = new List<UIVertex>();
    private float _fElapseTime;
    Color _sColorOrigin;

    // ========================================================================== //

    /* public - [Do~Something] Function 	        */

    static float Calculate_Left_To_Right(float fMin, float fMax, float fCurrent, float fElapseTime)
    {
        float fCurXNormalized = Mathf.InverseLerp(fMin, fMax, fCurrent) - fElapseTime;
        fCurXNormalized = Mathf.PingPong(fCurXNormalized, 1f);

        return fCurXNormalized;
    }

    static float Calculate_Right_ToLeft(float fMin, float fMax, float fCurrent, float fElapseTime)
    {
        float fCurXNormalized = Mathf.InverseLerp(fMin, fMax, fCurrent) + fElapseTime;
        fCurXNormalized = Mathf.PingPong(fCurXNormalized, 1f);

        return fCurXNormalized;
    }


    public void DoDisable()
    {
        if (pGraphicTarget == null)
            Awake();

        eUpdateMode = EUpdateMode.Manual;
        pGraphicTarget.color = _sColorOrigin;
        pGraphicTarget.SetAllDirty();
    }

    public void DoUpdate(float fDeltaTime)
    {
        _fElapseTime += fDeltaTime * fSpeed;

        pGraphicTarget.SetAllDirty();
    }

    // ========================================================================== //

    /* protected - [Override & Unity API]       */

#if UNITY_EDITOR
    protected override void Reset()
    {
        GradientColorKey[] arrColorKey =
        {
            new GradientColorKey(Color.red, 0f),
            new GradientColorKey(Color.blue, 1f)
        };

        GradientAlphaKey[] arrAlphaKey =
        {
            new GradientAlphaKey(1f, 0f),
            new GradientAlphaKey(1f, 1f)
        };

        pGradient.SetKeys(arrColorKey, arrAlphaKey);
    }
#endif

    protected override void Awake()
    {
        pGraphicTarget = GetComponent<Graphic>();
        _sColorOrigin = pGraphicTarget.color;
    }

    private void Update()
    {
        switch (eUpdateMode)
        {
            case EUpdateMode.Time:
                DoUpdate(Time.deltaTime);
                break;

            case EUpdateMode.RealTime:
                DoUpdate(Time.unscaledDeltaTime);
                break;
        }
    }


    public override void ModifyMesh(VertexHelper pVertexHelper)
    {
        if (_fElapseTime == 0f)
            return;

        if (g_map_OnCalculateEvaluate.TryGetValue(eGradient_Direction, out OnCalculate_GradientEvaluate OnCalculateHow) == false)
        {
            Debug.LogError($"{name} - Error NotContain {nameof(g_map_OnCalculateEvaluate)} // eGradient_Direction: {eGradient_Direction}");
            return;
        }

        _listVertex.Clear();
        pVertexHelper.GetUIVertexStream(_listVertex);
        if (_listVertex.Count == 0) // Text의 경우 길이가 0이면 Vertex가 없을 수 있음
            return;

        float fMin = _listVertex.Min(p => p.position.x);
        float fMax = _listVertex.Max(p => p.position.x);

        for (int i = 0; i < _listVertex.Count; i++)
        {
            UIVertex sVertex = _listVertex[i];
            sVertex.color = pGradient.Evaluate(OnCalculateHow(fMin, fMax, sVertex.position.x, _fElapseTime));
            _listVertex[i] = sVertex;
        }

        pVertexHelper.Clear();
        pVertexHelper.AddUIVertexTriangleStream(_listVertex);
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    #endregion Private
}