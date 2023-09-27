//using UnityEngine;
//using System.Collections;
//using System;
//using UnityEngine.UI;
//using UnityEngine.Purchasing;
//using BizzyBeeGames;
//using BizzyBeeGames.Sudoku;

//public class PurchaseManager : MonoBehaviour, IStoreListener
//{

//    public static PurchaseManager Instance;

//    private static IStoreController m_StoreController;          // The Unity Purchasing system.
//    private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.

//    [SerializeField] private GameObject coinsIndicator;
//    //Key
//    private static string InAppGoogleKey = "";

//    [Header("IAP DATA")]
//    [SerializeField]
//    public iapData[] iapValues; 
   
//    public static string[] localPrices;


    
  
//    // Product identifiers for all products capable of being purchased: 
//    // "convenience" general identifiers for use with Purchasing, and their store-specific identifier 
//    // counterparts for use with and outside of Unity Purchasing. Define store-specific identifiers 
//    // also on each platform's publisher dashboard (iTunes Connect, Google Play Developer Console, etc.)

//    // General product identifiers for the consumable, non-consumable, and subscription products.
//    // Use these handles in the code to reference which product to purchase. Also use these values 
//    // when defining the Product Identifiers on the store. Except, for illustration purposes, the 
//    // kProductIDSubscription - it has custom Apple and Google identifiers. We declare their store-
//    // specific mapping to Unity Purchasing's AddProduct, below.
//    public string[] kProductIDConsumable = new string[6];


//    private void Awake()
//    {
//        if (Instance == null)
//            Instance = this;
//    }

//    void Start()
//    {
//        // If we haven't set up the Unity Purchasing reference
//        if (m_StoreController == null)
//        {
//            // Begin to configure our connection to Purchasing
//            InitializePurchasing();
//        }
//    }

//    public void InitializePurchasing()
//    {
//        // If we have already connected to Purchasing ...
//        if (IsInitialized())
//        {
//            // ... we are done here.
//            return;
//        }

//        // Create a builder, first passing in a suite of Unity provided stores.
//        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
//#if UNITY_ANDROID
//        builder.Configure<IGooglePlayConfiguration>().SetPublicKey(InAppGoogleKey);
//#endif


//        // Add a product to sell / restore by way of its identifier, associating the general identifier
//        // with its store-specific identifiers.
//        for (int i = 0; i < kProductIDConsumable.Length-1; ++i)
//        {
//            builder.AddProduct(kProductIDConsumable[i], ProductType.Consumable, new IDs(){
//                { kProductIDConsumable[i], AppleAppStore.Name },
//                { kProductIDConsumable[i], GooglePlay.Name },
//            });
//        }
//        //Debug.Log("");
//        builder.AddProduct(kProductIDConsumable[kProductIDConsumable.Length - 1], ProductType.NonConsumable, new IDs(){
//                { kProductIDConsumable[kProductIDConsumable.Length-1], AppleAppStore.Name },
//                { kProductIDConsumable[kProductIDConsumable.Length-1], GooglePlay.Name },
//            });

//        // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
//        // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
//        UnityPurchasing.Initialize(this, builder);
//    }


//    private bool IsInitialized()
//    {
//        // Only say we are initialized if both the Purchasing references are set.
//        return m_StoreController != null && m_StoreExtensionProvider != null;
//    }

       
//    public void BuyProductID(string productId)
//    {
//        // If Purchasing has been initialized ...
//        if (IsInitialized())
//        {
//            // ... look up the Product reference with the general product identifier and the Purchasing 
//            // system's products collection.
//            Product product = m_StoreController.products.WithID(productId);

//            // If the look up found a product for this device's store and that product is ready to be sold ... 
//            if (product != null && product.availableToPurchase)
//            {
//                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
//                // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
//                // asynchronously.
//                m_StoreController.InitiatePurchase(product);
//            }
//            // Otherwise ...
//            else
//            {
//                // ... report the product look-up failure situation  
//                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
//            }
//        }
//        // Otherwise ...
//        else
//        {
//            // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
//            // retrying initiailization.
//            Debug.Log("BuyProductID FAIL. Not initialized.");
//        }
//    }


//    // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
//    // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
//    public void RestorePurchases()
//    {
//        // If Purchasing has not yet been set up ...
//        if (!IsInitialized())
//        {
//            // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
//            Debug.Log("RestorePurchases FAIL. Not initialized.");
//            return;
//        }

//        // If we are running on an Apple device ... 
//        if (Application.platform == RuntimePlatform.IPhonePlayer ||
//            Application.platform == RuntimePlatform.OSXPlayer)
//        {
//            // ... begin restoring purchases
//            Debug.Log("RestorePurchases started ...");

//            // Fetch the Apple store-specific subsystem.
//            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
//            // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
//            // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
//            apple.RestoreTransactions((result) =>
//            {
//                // The first phase of restoration. If no more responses are received on ProcessPurchase then 
//                // no purchases are available to be restored.
//                Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
//            });
//        }
//        // Otherwise ...
//        else
//        {           
//            //ScreenManager.Instance.mainMenu.gameObject.GetComponent<MainScreen>().CheckPurchaseButtons();
//            PopupManager.Instance.coinsPopup.CheckNoAdsPurchased();
//            // We are not running on an Apple device. No work is necessary to restore purchases.
//            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
//        }
//    }

//    IEnumerator CallNativeWithDelay()
//    {
//        yield return new WaitForSeconds(1.5f);
//        PopupManager.Instance.CustomNativePopup("Purchased", "All Ads Removed.");
//    }

//    //  
//    // --- IStoreListener
//    //

//    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
//    {
//        // Purchasing has succeeded initializing. Collect our Purchasing references.
//        Debug.Log("OnInitialized: PASS");

//        // Overall Purchasing system, configured with products for this application.
//        m_StoreController = controller;
//        // Store specific subsystem, for accessing device-specific store features.
//        m_StoreExtensionProvider = extensions;

        
//        localPrices = new string[m_StoreController.products.all.Length];
//        //localPriceValues = new float[m_StoreController.products.all.Length];
//        // Populate the product menu now that we have Products
//        for (int t = 0; t < m_StoreController.products.all.Length; t++)
//        {
//            var item = m_StoreController.products.all[t];
//            var description = string.Format("{0} - {1}", item.metadata.localizedTitle, item.metadata.localizedPriceString);
//            localPrices[t] = item.metadata.localizedPriceString;
//            //localPriceValues[t] = (float)item.metadata.localizedPrice;
//            //inAppData.text = "\n" + item.metadata.localizedPriceString.ToString ();			
//        }

//        //if (m_StoreController.products.WithID(kProductIDConsumable[kProductIDConsumable.Length - 1]).hasReceipt)
//        //{
//        //    PlayerPrefs.SetInt("SHOW_ADS", 0);
//        //    ScreenManager.Instance.mainMenu.gameObject.GetComponent<MainScreen>().CheckPurchaseButtons();
//        //    PopupManager.Instance.coinsPopup.CheckNoAdsPurchased();
//        //}
//    }


//    public void OnInitializeFailed(InitializationFailureReason error)
//    {
//        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
//        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
//    }


//    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
//    {
//        // A consumable product has been purchased by this user.
//        if (String.Equals(args.purchasedProduct.definition.id, kProductIDConsumable[0], StringComparison.Ordinal))
//        {
//            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
//            PurchasedProduct(0);
//        }
//        else if (String.Equals(args.purchasedProduct.definition.id, kProductIDConsumable[1], StringComparison.Ordinal))
//        {
//            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
//            PurchasedProduct(1);
//        }
//        else if (String.Equals(args.purchasedProduct.definition.id, kProductIDConsumable[2], StringComparison.Ordinal))
//        {
//            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
//            PurchasedProduct(2);
//        }
//        else if (String.Equals(args.purchasedProduct.definition.id, kProductIDConsumable[3], StringComparison.Ordinal))
//        {
//            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
//            PurchasedProduct(3);
//        }
//        else if (String.Equals(args.purchasedProduct.definition.id, kProductIDConsumable[4], StringComparison.Ordinal))
//        {
//            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
//            PurchasedProduct(4);
//        }
//        else if (String.Equals(args.purchasedProduct.definition.id, kProductIDConsumable[5], StringComparison.Ordinal))
//        {
//            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
//            PurchasedNonConsumable();
//        }
//        else
//        {
//            Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
//        }

//        // Return a flag indicating whether this product has completely been received, or if the application needs 
//        // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
//        // saving purchased products to the cloud, and when that save is delayed. 
//        return PurchaseProcessingResult.Complete;
//    }


//    void PurchasedNonConsumable()
//    {
//        Debug.Log("No ads Purchased!");

//        AdsManager.Instance.RemoveAdsValueSet();
//       // ScreenManager.Instance.mainMenu.gameObject.GetComponent<MainScreen>().CheckPurchaseButtons();
//        PopupManager.Instance.coinsPopup.CheckNoAdsPurchased();
//        ScreenManager.Instance.mainScreen.CheckPurchaseButtons();
//        PopupManager.Instance.CheckPurRemoveAdsPanel();

//        PlayerPrefs.SetInt("PURCHASE_COUNT", PlayerPrefs.GetInt("PURCHASE_COUNT", 0) + 1);
//        PlayerPrefs.SetFloat("TOTAL_PURCHASE_PRICE", PlayerPrefs.GetFloat("TOTAL_PURCHASE_PRICE", 0f) + iapValues[iapValues.Length-1].price);
//        GameAnalytics.PurchasedCount(PlayerPrefs.GetInt("PURCHASE_COUNT", 0), PlayerPrefs.GetFloat("TOTAL_PURCHASE_PRICE", 0f));
//    }

//    void PurchasedProduct(int i)
//    {        
//        PopupManager.Instance.Show("rewarded", new object[] { iapValues[i].amount, "Pcoins" });
//        coinsIndicator.SetActive(true);
//        //CurrencyManager.Instance.GiveCoins(iapValues[i].amount);
//        CurrencyManager.Instance.Give("coins", iapValues[i].amount, 0.5f, true);
//        CurrencyManager.Instance.isPurchasedIAP = true;
//        GameAnalytics.EarnVirtualCurrency("Coins", iapValues[i].amount);

//        //Analytics
//        PlayerPrefs.SetInt("PURCHASE_COUNT", PlayerPrefs.GetInt("PURCHASE_COUNT", 0) + 1);
//        PlayerPrefs.SetFloat("TOTAL_PURCHASE_PRICE", PlayerPrefs.GetFloat("TOTAL_PURCHASE_PRICE", 0f) + iapValues[i].price);
        
//        GameAnalytics.PurchasedCount(PlayerPrefs.GetInt("PURCHASE_COUNT", 0), PlayerPrefs.GetFloat("TOTAL_PURCHASE_PRICE", 0f));
//       // AdsManager.Instance.RemoveAdsValueSet();
//    }

//    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
//    {
//        if (Application.internetReachability != NetworkReachability.NotReachable)
//        {

//            PlayerPrefs.SetInt("PURCHASE_FAILED_COUNT", PlayerPrefs.GetInt("PURCHASE_FAILED_COUNT", 0) + 1);
//            GameAnalytics.PurchasedFailedCount(PlayerPrefs.GetInt("PURCHASE_FAILED_COUNT", 0));

//#if UNITY_ANDROID
//            NativeMessage dialog = new NativeMessage("PURCHASE FAIL", failureReason.ToString());
//#elif UNITY_IOS
//        //MobileNativePopups.OpenAlertDialog(
//        //        "PURCHASE FAIL", failureReason.ToString(),
//        //        "OK",
//        //        () => { Debug.Log("Ok was pressed"); });
//#endif

//        }
//        return;
//        // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
//        // this reason with the user to guide their troubleshooting actions.
//        //Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
//    }
//}

//[Serializable]
//public class iapData
//{
//    public int amount;
//    public float price;
//}

