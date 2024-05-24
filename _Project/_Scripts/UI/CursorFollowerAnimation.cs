using Febucci.UI.Core;
using Febucci.UI.Effects;
using TMPro;
using UnityEngine;

    [CreateAssetMenu(fileName = "Cursor Follower Animation", menuName = "Live/Animations/Behaviors/Cursor Follower")]
    public class CursorFollowerAnimation : AnimationScriptableBase
    {
        [SerializeField] Vector2 distanceRanges = new Vector2(.2f, 1f);

        [SerializeField] bool inverted = true;
        
        bool camExists;
        Camera cam;
        Vector2 cursorWorldPos;
        bool isWorldSpace;
        public override void ResetContext(TAnimCore animator)
        {
            cam = Camera.main;
            camExists = cam;
            if (!camExists) return;
            isWorldSpace = !animator.TryGetComponent(out TextMeshProUGUI result);
            cursorWorldPos = isWorldSpace ? cam.ScreenToWorldPoint(Input.mousePosition) : Input.mousePosition;
        }

        public override float GetMaxDuration() => -1;

        public override bool CanApplyEffectTo(CharacterData character, TAnimCore animator) => camExists;
        
        public override void ApplyEffectTo(ref CharacterData character, TAnimCore animator)
        {
            Vector2 letterPos = animator.transform.TransformPoint(character.source.positions.GetMiddlePos());
            if (!isWorldSpace) letterPos = cam.ScreenToWorldPoint(letterPos);

            var dir = (cursorWorldPos - letterPos);
            float pct = Mathf.Clamp01((dir.magnitude - distanceRanges.x)/(distanceRanges.y-distanceRanges.x));
            if(inverted) pct = 1 - pct;
            
            character.current.positions.MoveChar(-dir.normalized * pct * 2);
        }
    }

