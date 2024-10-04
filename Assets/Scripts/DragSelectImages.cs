using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragSelectImages : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private List<Image> selectedImages = new List<Image>();
    private Color originalColor = Color.white;
    private Color selectedColor = Color.green;
    
    // Called when the mouse or finger touches the image
    public void OnPointerDown(PointerEventData eventData)
    {
        Image image = GetImageUnderPointer(eventData);
        if (image != null)
        {
            SelectImage(image);
        }
    }

    // Called when the mouse or finger is dragged over other images
    public void OnDrag(PointerEventData eventData)
    {
        Image image = GetImageUnderPointer(eventData);
        if (image != null && !selectedImages.Contains(image))
        {
            SelectImage(image);
        }
    }

    // Called when the mouse or finger is released
    public void OnPointerUp(PointerEventData eventData)
    {
        ResetAllImages();
    }

    // Selects an image by turning it green
    private void SelectImage(Image image)
    {
        selectedImages.Add(image);
        image.color = selectedColor;
    }

    // Resets all images back to their original color
    private void ResetAllImages()
    {
        foreach (var img in selectedImages)
        {
            img.color = originalColor;
        }
        selectedImages.Clear();
    }

    // Gets the UI Image under the pointer (mouse/finger)
    private Image GetImageUnderPointer(PointerEventData eventData)
    {
        GameObject hoveredObject = eventData.pointerEnter;
        if (hoveredObject != null)
        {
            return hoveredObject.GetComponent<Image>();
        }
        return null;
    }
}