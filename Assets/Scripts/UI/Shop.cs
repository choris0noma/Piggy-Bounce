using CubeHopper.Game;
using CubeHopper.SavingData;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CubeHopper.UI
{
    public class Shop : MonoBehaviour
    {
        private const string FILE_PATH = "/s_data.json";
        [SerializeField] private GameObject shopPanel;
        [SerializeField] private PlayerDataManager _playerData; 
        [SerializeField] private SkinItem[] skinList;
        [SerializeField] private ShopPanel[] skinPanels;

        private Vector2 _startPosition;
        private Vector2 _endPosition = Vector2.zero;
        private IDataService _dataService = new JsonDataService();
        private List<int> _purchasedItems = new List<int> { 0 };
        private Player _player;
        private void Awake()
        {
            _startPosition = UnityEngine.Camera.main.ScreenToWorldPoint(new Vector2(Screen.width/2, -Screen.height/2));
            shopPanel.transform.position = _startPosition;
            _player = FindObjectOfType<Player>();
        }
        private void Start()
        {
            LoadPurchasedSkinsData();
            LoadSkins();
            AddButtonEvents();
            LoadPurchasedSkins();
            LoadCurrentSkin();
            CheckAvailableSkins();
        }
        private void AddButtonEvents()
        {
            for (int i =0; i< skinList.Length; i++)
            {
                skinPanels[i].AddButtonEvent(this, i);
            }
        }
        private void LoadCurrentSkin()
        {
            int currentSkin = PlayerPrefs.GetInt("CurrentSkin", 0);
            
            if (!_purchasedItems.Contains(currentSkin))
            {
                currentSkin = 0;
            }

            skinPanels[currentSkin].isEquipped = true;
            _player.SetSkin(skinList[currentSkin].IdleSprite, skinList[currentSkin].FlySprite);
        }
        private void LoadSkins()
        {
            for (int i = 0; i < skinList.Length; i++) 
            {
                skinPanels[i].Price = skinList[i].Price;
                skinPanels[i].Image = skinList[i].IdleSprite;
            }
        }
        private void LoadPurchasedSkins()
        {
            foreach (int index in _purchasedItems)
            {
                skinPanels[index].isPurchased = true;
            }
        }
        private void CheckAvailableSkins()
        {
            for (int i = 0; i< skinList.Length; i++)
            {
                if (!_purchasedItems.Contains(i))
                {
                    skinPanels[i].isInteractable = skinList[i].Price <= _playerData.Money;
                }
            }
        }
        public void PurchaseSkin(int index)
        {
            int price = skinList[index].Price;
            if (_playerData.Money >= price)
            {
                _playerData.UpdateMoneyInfo(price);
                EquipeSkin(index);
                skinPanels[index].isPurchased = true;
                _purchasedItems.Add(index);
                SavePurchasedSkinsData();
            }
        }
        public void EquipeSkin(int index)
        {
            if (_purchasedItems.Contains(index))
            {
                PlayerPrefs.SetInt("CurrentSkin", index);
                _player.SetSkin(skinList[index].IdleSprite, skinList[index].FlySprite);
                foreach (ShopPanel skin in skinPanels) skin.isEquipped = false; 
                skinPanels[index].isEquipped = true;
            }
        }
        
        public void Open()
        {
            LeanTween.cancel(shopPanel);
            shopPanel.SetActive(true);
            shopPanel.transform.LeanMove(_endPosition, 0.5f).setEaseOutQuad();
        }
        public void Close()
        {
            LeanTween.cancel(shopPanel);
            shopPanel.transform.LeanMove(_startPosition, 0.5f).setEaseOutQuad().setOnComplete(() =>
            {
                shopPanel.SetActive(false);
            });
        }
        private void SavePurchasedSkinsData()
        {
            if (!_dataService.SaveData(FILE_PATH, _purchasedItems, true))
            {
                Debug.LogError("Cccc");
            }
        }

        private void LoadPurchasedSkinsData()
        {
            if (_dataService.CheckPathExistence(FILE_PATH))
            {
                _purchasedItems = _dataService.LoadData<List<int>>(FILE_PATH, true);
            }
            else 
            {
                SavePurchasedSkinsData();
            }
        }
    }
}
