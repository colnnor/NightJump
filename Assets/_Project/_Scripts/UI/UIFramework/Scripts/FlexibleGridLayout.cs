using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class FlexibleGridLayout : LayoutGroup
{
    public enum FitType
    {
        Uniform,
        Width,
        Height,
        FixedRows,
        FixedColumns
    }
    public enum SpacingType
    {
        Uniform,
        Individual
    }

    [Title("Fit Type")]
    [EnumToggleButtons]
    [HideLabel]
    [SerializeField] FitType fitType;

    [BoxGroup("Parent", ShowLabel = false)]
    [HideIf("fitType", Value = FitType.FixedColumns)]
    [MinValue(0)]
    [SerializeField] int rows;
    [BoxGroup("Parent", ShowLabel = false)]
    [HideIf("fitType", Value = FitType.FixedRows)]
    [MinValue(0)]
    [SerializeField] int columns;
    
    [Title("Spacing Type")]
    [EnumToggleButtons]
    [HideLabel]
    [SerializeField] SpacingType spacingType;
    
    [ShowIf("spacingType", Value = SpacingType.Individual)]
    [BoxGroup("Spacing", ShowLabel = false)]
    [SerializeField] Vector2 spacing;

    [ShowIf("spacingType", Value = SpacingType.Uniform)]
    [BoxGroup("Spacing", ShowLabel = false)]
    [LabelText("Spacing")]
    [SerializeField] float uniformSpacing;
    

    Vector2 cellSize;
    bool fitX, fitY;
    
    public override void CalculateLayoutInputVertical()
    {
        base.CalculateLayoutInputHorizontal();

        if(fitType == FitType.Width || fitType == FitType.Height || fitType == FitType.Uniform)
        {
            fitX = true;
            fitY = true;
           
            float sqrRt = Mathf.Sqrt(transform.childCount);
            rows = Mathf.CeilToInt(sqrRt);
            columns = Mathf.CeilToInt(sqrRt);
        }
         

        if (fitType == FitType.Width || fitType == FitType.FixedColumns)
        {
            rows = Mathf.CeilToInt(transform.childCount / (float)columns);
        }
        if (fitType == FitType.Height || fitType == FitType.FixedRows)
        {
            columns = Mathf.CeilToInt(transform.childCount / (float)rows);
        }
        if (fitType == FitType.Uniform)
        {
            rows = Mathf.CeilToInt(transform.childCount / (float)columns);

            columns = Mathf.CeilToInt(transform.childCount / (float)rows);

        }
        if (spacingType == SpacingType.Uniform)
        {
            spacing = new Vector2(uniformSpacing, uniformSpacing);
        }

        float parentWidth = rectTransform.rect.width;
        float parentHeight = rectTransform.rect.height;

        float cellWidth = (parentWidth / (float)columns) - ((spacing.x / (float)columns) * (columns - 1)) - (padding.left / (float)columns) - (padding.right / (float)columns);
        float cellHeight = (parentHeight / (float)rows) - ((spacing.y / (float)rows) * (columns - 1)) - (padding.top / (float)rows) - (padding.bottom / (float)rows) ;

        cellSize.x = fitX ? cellWidth : cellSize.x;
        cellSize.y = fitY ? cellHeight : cellSize.y;

        int columnCount = 0;
        int rowCount = 0;

        for (int i = 0; i < rectChildren.Count; i++)
        {
            rowCount = i / columns;
            columnCount = i % columns;

            if (fitType == FitType.Height)
            {
                var rest = (rows * columns) - i;
                if (i == rectChildren.Count - 1 && rest > 0)
                {
                    cellSize.x = cellSize.x * rest + spacing.x;
                    columnCount /= rest;
                }
            }


            var item = rectChildren[i];

            var xPos = (cellSize.x * columnCount) + (spacing.x * columnCount) + padding.left;
            var yPos = (cellSize.y * rowCount) + (spacing.y * rowCount) + padding.top;

            SetChildAlongAxis(item, 0, xPos, cellSize.x);
            SetChildAlongAxis(item, 1, yPos, cellSize.y);
        }

    }

    public override void SetLayoutHorizontal()
    {

    }

    public override void SetLayoutVertical()
    {

    }

}
