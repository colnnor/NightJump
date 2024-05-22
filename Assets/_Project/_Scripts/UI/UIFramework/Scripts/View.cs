using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

[System.Serializable]
[InlineEditor]
public struct ContainerImage
{
    public UIStyle style;
    public GameObject container;
    public Image image;
    public ContainerImage(GameObject container, Image image, UIStyle style)
    {
        this.style = style;
        this.container = container;
        this.image = image;
    }
}
public class View : CustomUIComponent
{
    [Title("Layoutgroup Data")]
    [SerializeField] private ViewSO viewData;

    [Title("CustomUIComponent Elements")]
    [SerializeField] private List<ContainerImage> containers = new();

    private VerticalLayoutGroup verticalLayoutGroup;

    public override void Setup()
    {
        verticalLayoutGroup = GetComponent<VerticalLayoutGroup>();

        foreach (Transform child in transform)
        {
            if (containers.Any(container => container.container == child.gameObject))
            {
                continue;
            }

            GameObject go = child.gameObject;
            Image image = go.GetComponent<Image>();
            ContainerImage containerImage = new ContainerImage(go, image, UIStyle.Primary);
            containers.Add(containerImage);
        }
    }
    public override void Configure()
    {
        if (!viewData) return;

        verticalLayoutGroup.padding = viewData.padding;
        verticalLayoutGroup.spacing = viewData.spacing;

        foreach (ContainerImage containerImage in containers)
        {
            //containerImage.image.color = viewData.theme.GetBGColor(containerImage.style);
        }
    }
}
