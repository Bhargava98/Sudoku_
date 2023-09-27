using BizzyBeeGames.Sudoku;
using MiniJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using VoxelBusters.NativePlugins;

namespace BizzyBeeGames
{
    public class PopupManager : SingletonComponent<PopupManager>
    {
        #region Classes

        [System.Serializable]
        private class PopupInfo
        {
            [Tooltip("The popups id, used to show the popup. Should be unique between all other popups.")]
            public string popupId = "";

            [Tooltip("The Popup component to show.")]
            public Popup popup = null;
        }
        
        #endregion

        #region Inspector Variables

        [SerializeField] private List<PopupInfo> popupInfos = null;
        public CoinShopPopup coinsPopup = null;
        public Sprite[] rewardImage = null;
        #endregion

        #region Member Variables

        public List<Popup> activePopups;
      
        public int NotificationID;



        //Cross Promo Added
        [Header("Cross Promo")] //v_0.5
        public static bool crosspromo_Active = false;
        public static string crossPromo_UTM_Medium = null;
        public static string crossPromo_PackageID = null;
        public static string crossPromo_UTM_Source = null;
        public static int crosspromo_ClickCount = 0;
        public static string crossPromo_uniqueIndex = "";

        public CrossPromoIcons[] promoIcons;

        [SerializeField] GameObject crossPromoButton;
        [SerializeField] Image crosspromo_Icon;
        #endregion

        #region Unity Methods

        protected override void Awake()
        {
            base.Awake();


            Input.multiTouchEnabled = false;

            activePopups = new List<Popup>();

            for (int i = 0; i < popupInfos.Count; i++)
            {
                popupInfos[i].popup.Initialize();
            }
        }


        private void Start()
        {
            if (Application.internetReachability != NetworkReachability.NotReachable)
            { StartCoroutine(GetCrossPlatformData()); }


          

        }

        IEnumerator GetCrossPlatformData()//v_0.5
        {
            yield return new WaitForSeconds(0.3f);
            if (UnityRemoteData.crossPromoData != null)
            {
                GetCrosspromoValues(UnityRemoteData.crossPromoData);
            }
        }
        #endregion

        #region Public Methods

        public void ClearActivePopups()
        {
            activePopups.Clear();
        }

        public void Show(string id)
        {
            Show(id, null, null);
        }

        public void Show(string id, object[] inData)
        {
            Show(id, inData, null);
        }

        public void Show(string id, object[] inData, Popup.PopupClosed popupClosed)
        {
            Popup popup = GetPopupById(id);

            if (popup != null)
            {
                if (popup.Show(inData, popupClosed))
                {
                    activePopups.Add(popup);
                }
            }
            else
            {
                Debug.LogErrorFormat("[PopupController] Popup with id {0} does not exist", id);
            }
        }

        public void HidePopup(string id)
        {
            Popup popup = GetPopupById(id);

            popup.Hide(false);
        }

        
        public bool CloseActivePopup()
        {
            //print("Active Popups:"+ activePopups.Count);
            
            if (activePopups.Count > 0)
            {
                int index = activePopups.Count - 1;

                Popup popup = activePopups[index];

                if (popup.CanAndroidBackClosePopup)
                {
                    if (popup.gameObject.name == "PauseMenuPopup")
                    {
                        GameManager.Instance.UnpauseGame();
                    }
                    popup.Hide(true);

                    //activePopups.Clear();
                }
                return true;
            }

            return false;
        }

        public Text buyButtonText;
        public Text priceTag,priceDesc;
        public Button buyButton;
        public void CheckPurRemoveAdsPanel()
        {
            //if (PurchaseManager.localPrices != null)
            //{
            //    priceDesc.text = PurchaseManager.localPrices[5];
            //    priceTag.text =  PurchaseManager.localPrices[5];
            //}

            if (PlayerPrefs.GetInt("SHOW_ADS", 0) == 1)
            {
                //RestoreButton.SetActive(true);
                buyButtonText.text = "Purchased";
                buyButton.interactable = false;
            }
            else
            {
                //RestoreButton.SetActive(false);
                //buyButtonText.text = PurchaseManager.localPrices[5];
                buyButton.interactable = true;
            }
        }

        

        public void CustomNativePopup(string title, string msg)
        {
#if UNITY_ANDROID
            NativeMessage dialog = new NativeMessage(title, msg);
            //Debug.Log("Network Failed...can't Connect");
#elif UNITY_IOS
            //MobileNativePopups.OpenAlertDialog(
            //"Network", msg,
            //"OK", 
            //() => { Debug.Log("Accept was pressed"); });
           // Debug.log("Network Failed.. cant Connect");
#endif
        }

        public bool NoInternetNativePopup(string title, string msg)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
#if UNITY_ANDROID
                NativeMessage dialog = new NativeMessage(title, msg);
                //Debug.Log("Network Failed...can't Connect");
#elif UNITY_IOS
            //MobileNativePopups.OpenAlertDialog(
            //"Network", msg,
            //"OK", 
            //() => { Debug.Log("Accept was pressed"); });
            //Debug.log("Network Failed.. cant Connect");
#endif
                return true;
            }

            return false;
        }

        public void RateUsClicked()
        {
            if (NoInternetNativePopup("Network", "No Internet Access") == false)
            {

            }
        }

        public bool isInterst = false;
        public void OnPopupHiding(Popup popup)
        {
            //ClearActivePopups();  

            for (int i = activePopups.Count - 1; i >= 0; i--)
            {
                if (popup == activePopups[i])
                {
                    if (ScreenManager.Instance.CurrentScreenId == "game" && i == 0)
                    {
                        
                        GameManager.Instance.IsPaused = false;

                        if (!isInterst)
                        {
                            //AdsManager.Instance.ShowBanner(out bool u);                            
                        } 
                    }
                    //if (ScreenManager.Instance.CurrentScreenId == "game" )
                    //{

                    //    //if (activePopups.Count >= 1)
                    //    //{
                    //    //    GameManager.Instance.IsPaused = true;
                    //    //}


                    //    //if (i == 0)
                    //    //if (activePopups[i].gameObject.name == "HintShopPopup" || activePopups[i].gameObject.name == "CoinShopPopup" ||
                    //    //        activePopups[i].gameObject.name == "HowToPlayPopup")
                    //    //{
                    //    //    GameManager.Instance.IsPaused = false;
                                
                    //    //}

                    //    //if (i == 1 && activePopups.Count == 2)
                    //    //{
                    //    //    if (activePopups[i].gameObject.name == "HowToPlayPopup")
                    //    //        GameManager.Instance.IsPaused = true;
                    //    //}
                    //    //if (i == 0)
                    //    //    if (activePopups[i].gameObject.name == "GameOverPopup")
                    //    //    {
                    //    //        GameManager.Instance.IsPaused = false;
                    //    //    }

                       
                    //    //SelectThemesPopup
                    //}
                    
                    activePopups.RemoveAt(i);

                    //Debug.Log("PopupCount: "+activePopups.Count);

                    if (ScreenManager.Instance.CurrentScreenId == "game")
                    {
                        if (activePopups.Count >= 1)
                        {
                            GameManager.Instance.IsPaused = true;
                        }
                        else
                            GameManager.Instance.IsPaused = false;
                    }
                    

                        break;
                }
            }


        }

        public void ShowRewardsPopup(string reward)
        {
            //string[] reward = rewards.Split('-');
            Sprite image = null;

            switch (reward)
            {
                case "coinsDM":
                    //image = rewardImage[0];                           
                    //AdsManager.Instance.ShowRewardedAd(new object[] { image, reward });
                    //Instance.Show("rewarded", new object[] { image, reward });
                    //SoundManager.Instance.Play("reward");                       
                    break;
                case "coinsHints":
                    image = rewardImage[1];
                    if (CurrencyManager.Instance.coinsToBuyHints > CurrencyManager.Instance.GetAmount("coins"))
                    {
                        PopupManager.Instance.Show("coin_shop");
                        return;
                    }
                    //SoundManager.Instance.Play("reward");
                    Instance.Show("rewarded", new object[] { image, reward });
                    break;
                case "Adcoins":
                    if (NoInternetNativePopup("Network", "No Internet Access") == false)
                    {
                        image = rewardImage[0];

                        //AdsManager.Instance.ShowRewardedAd(new object[] { image, reward });
                        // Instance.Show("rewarded", new object[] { image, reward });
                        //SoundManager.Instance.Play("reward");
                    }
                    break;
                case "Adhints":
                    if (NoInternetNativePopup("Network", "No Internet Access") == false)
                    {
                        image = rewardImage[1];

                        //AdsManager.Instance.ShowRewardedAd(new object[] { image, reward });
                        //Instance.Show("rewarded", new object[] { image, reward });
                        //SoundManager.Instance.Play("reward");
                    }
                    break;
                case "Pcoins":
                    if (NoInternetNativePopup("Network", "No Internet Access") == false)
                    {
                        //image = rewardImage[0];                            
                        //Instance.Show("rewarded", new object[] { image, reward });
                        //SoundManager.Instance.Play("reward");
                    }
                    break;
            }
        }

        public void ShareURLOnSocialNetwork()
        {
            //NativeShare share = new NativeShare();


            // Show composer
            
        }
        #endregion

        #region Private Methods

        private Popup GetPopupById(string id)
        {
            for (int i = 0; i < popupInfos.Count; i++)
            {
                if (id == popupInfos[i].popupId)
                {
                    return popupInfos[i].popup;
                }
            }

            return null;
        }


        // private const string infoText = "{\"data\":[{\"Active\":true,\"PackageID\":\"Spots Connect\",\"UTM_Source\":\"jgdjgjgjfgjgfjh.com\",\"UTM_Medium\":\"jgdjgjgjfgjgfjh.com\",\"unique_index\":\"New6,11Dec2019\",\"MaxCount\":5}]}";
        /// <summary>
        /// Cross Platform get Promo Values
        /// </summary>
        /// <param name="json_info"></param>
        public void GetCrosspromoValues(string json_info)//v_0.5
        {
            if (json_info != null)
            {
                Dictionary<string, object> testData = Json.Deserialize(json_info) as Dictionary<string, object>;
                List<object> data = testData["data"] as List<object>;
                //  print(json_info);
                for (int i = 0; i < data.Count; i++)
                {
                    crosspromo_Active = (bool)(((Dictionary<string, object>)data[i])["Active"]);
                    crossPromo_PackageID = ((Dictionary<string, object>)data[i])["PackageID"].ToString();
                    crosspromo_ClickCount = int.Parse(((Dictionary<string, object>)data[i])["MaxCount"].ToString());
                    crossPromo_UTM_Source = ((Dictionary<string, object>)data[i])["UTM_Source"].ToString();
                    crossPromo_UTM_Medium = ((Dictionary<string, object>)data[i])["UTM_Medium"].ToString();
                    crossPromo_uniqueIndex = ((Dictionary<string, object>)data[i])["unique_index"].ToString();
                }

                CheckCrossPromo();
            }
        }

        /// <summary>
        /// check cross platform promo
        /// </summary>
        public void CheckCrossPromo()//v_0.5
        {
            crossPromoButton.SetActive(false);

            if (Application.internetReachability != NetworkReachability.NotReachable) //Enable only if net available
            {
                bool flag = false;
                if (crosspromo_Active && PlayerPrefs.GetInt("CP_" + crossPromo_uniqueIndex, 0) < crosspromo_ClickCount)
                {
                    for (int i = 0; i < promoIcons.Length; i++)
                    {
                        if (promoIcons[i].packName == crossPromo_PackageID)
                        {
                            crosspromo_Icon.sprite = promoIcons[i].icon;
                            i = promoIcons.Length;
                            flag = true;
                            break;
                        }
                    }
                    if (flag)
                        crossPromoButton.SetActive(true);
                }
            }
        }

        public void OnCrossPromoButtonClick()//v_0.5
        {

            PlayerPrefs.SetInt("CP_" + crossPromo_uniqueIndex, PlayerPrefs.GetInt("CP_" + crossPromo_uniqueIndex, 0) + 1);
            
            
            Application.OpenURL("https://play.google.com/store/apps/details?id=" + crossPromo_PackageID + "&referrer=utm_source%3D" + crossPromo_UTM_Source + "utm_medium%3D" + crossPromo_UTM_Medium);
            CheckCrossPromo();
        }
        #endregion
    }


    [System.Serializable]
    public class CrossPromoIcons
    {
        public string packName;
        public Sprite icon;
    }

}