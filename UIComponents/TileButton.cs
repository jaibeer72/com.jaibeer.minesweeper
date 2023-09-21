using System.Collections;
using System; 
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public enum Tile_Status
{
    Closed,
    Numbered,
    Empty,
    Bomb,
    Flaged
};

public class TileButton : Button
{
    public Action<TileButton> TileButtonClicked;
    public Action<TileButton> TileButtonRightClicked; 

    public TileButton()
    {
        this.clicked += TileButton_Clicked;

        this.RegisterCallback<MouseUpEvent>(OnTileButtonRightClicked);
    }

    public TileButton(int LocX, int LocY)
    {
        Loc_X = LocX;
        Loc_Y = LocY;

        this.clicked += TileButton_Clicked;

        this.RegisterCallback<MouseUpEvent>(OnTileButtonRightClicked);
    }

    private void OnTileButtonRightClicked(MouseUpEvent evt)
    {
        if(evt.button == (int)MouseButton.RightMouse)
        {
            TileButtonRightClicked?.Invoke(this);
        }
    }

    private void TileButton_Clicked()
    {
        TileButtonClicked?.Invoke(this); 
    }

    public int Loc_X, Loc_Y;

    private Tile_Status m_Status;
    public Tile_Status status
    {
        get { return m_Status; }
        set
        {
            if (m_Status != value)
            {
                m_Status = value;
                ChangeTileStatus();
            }
        }
    }

    private string flagImagePath = "Packages/com.jaibeer.minesweeper/Assets/Unknown.jpeg";
    private string mineImagePath = "Packages/com.jaibeer.minesweeper/Assets/Bomb.png";

    public new class UxmlFactory : UxmlFactory<TileButton, UxmlTraits> { }

    public new class UxmlTraits : Button.UxmlTraits
    {
        //UxmlTypeAttributeDescription<Image> m_ClosedImage = new() { name = "closed-image" };
        UxmlEnumAttributeDescription<Tile_Status> m_Status = new UxmlEnumAttributeDescription<Tile_Status> { name = "Tile-Status" };

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            ((TileButton)ve).status = m_Status.GetValueFromBag(bag, cc);
        }
    }
    public void SetImage(string addressableAssetKey)
    {
        Addressables.LoadAssetAsync<Sprite>(addressableAssetKey).Completed += OnImageLoaded;
    }

    private void OnImageLoaded(AsyncOperationHandle<Sprite> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            var styleBackgroundImage = new StyleBackground(obj.Result);
            this.style.backgroundImage = styleBackgroundImage;
        }
        else
        {
            Debug.LogError("Failed to load image from addressables");
        }
    }

    public void SetText(string text)
    {
        this.text = text;
    }

    private void ChangeTileStatus()
    {
        switch (status)
        {
            case Tile_Status.Closed:
                this.style.backgroundImage = null;
                SetText(""); // No text for closed tiles
                break;
            case Tile_Status.Numbered:
                this.style.backgroundImage = null;
                this.style.backgroundColor = Color.white;
                SetText("1"); // Replace with the actual number
                break;
            case Tile_Status.Empty:
                this.style.backgroundImage = null;
                this.style.backgroundColor = Color.white; 
                SetText(""); // No text for empty tiles
                break;
            case Tile_Status.Bomb:
                SetImage(mineImagePath);
                SetText(""); // No text for bomb tiles
                break;
            case Tile_Status.Flaged:
                SetImage(flagImagePath);
                SetText("F"); // "F" for flagged
                break;
        }
    }
}
