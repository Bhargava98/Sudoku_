using BizzyBeeGames.Sudoku;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BizzyBeeGames
{
    public class CurrencyManager : SingletonComponent<CurrencyManager>, ISaveable
    {
        #region Classes

        [System.Serializable]
        private class CurrencyInfo
        {
            public string id = "";
            public int startingAmount = 0;
            public bool showNotEnoughPopup = false;
            public string popupTitleText = "";
            public string popupMessageText = "";
            public bool popupHasStoreButton = false;
            public bool popupHasRewardAdButton = false;
            public string rewardButtonText = "";
            public int rewardAmount = 0;
        }

        #endregion

        #region Inspector Variables

        [SerializeField] private string notEnoughPopupId = "";
        [SerializeField] private List<CurrencyInfo> currencyInfos = null;
        [SerializeField] private GameObject coinPref;
        [SerializeField] private GameObject gamePlayCoinsEndPoint;

        public int hintsAwardedPerBuy = 10;
        public int coinsToBuyHints = 1200;
        
        [SerializeField] private List<Text> coinTexts;

        private GameObject startObj, endObj = null;
        [HideInInspector] public bool isPurchasedIAP = false;
        #endregion

        #region Member Variables

        private Dictionary<string, int> currencyAmounts;

        #endregion

        #region Properties

        public string SaveId { get { return "currency_manager"; } }

        public System.Action<string> OnCurrencyChanged { get; set; }

        #endregion

        #region Unity Methods

        protected override void Awake()
        {
            base.Awake();

            currencyAmounts = new Dictionary<string, int>();

            SaveManager.Instance.Register(this);

            if (!LoadSave())
            {
                SetStartingValues();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the amount of currency the player has
        /// </summary>
        public int GetAmount(string currencyId)
        {
            if (!CheckCurrencyExists(currencyId))
            {
                return 0;
            }

            return currencyAmounts[currencyId];
        }
        public void GiveCoins(int amount,float animDelay,bool isAnim)
        {
            if (amount < 0 && Mathf.Abs(amount) > currencyAmounts["coins"])
            {
                return;
            }
            Give("coins", amount,animDelay,isAnim);
        }

        public void GiveCoins(int amount)
        {
            if (amount < 0 && Mathf.Abs(amount) > currencyAmounts["coins"])
            {
                return;
            }
            Give("coins", amount);
        }
        public void GiveCoinsAndHints(int amount)
        {
            if (amount < 0 && Mathf.Abs(amount) > currencyAmounts["coins"])
            {
                return;
            }
            Give("coins", amount);
            Give("hints", hintsAwardedPerBuy);

            //GameAnalytics.SpendVirtualCurrency("Spend to buy Hints",1,"Coins",Mathf.Abs(amount));
            //GameAnalytics.EarnVirtualCurrency("Bought Hints with Coins",hintsAwardedPerBuy);
        }

        public void GiveHints(int amount)
        {
            if (amount < 0 && Mathf.Abs(amount) > currencyAmounts["hints"])
            {
                return;
            }
            Give("hints", amount);
        }
        /// <summary>
        /// Tries to spend the curreny
        /// </summary>
        public bool TrySpend(string currencyId, int amount)
        {
            if (!CheckCurrencyExists(currencyId))
            {
                return false;
            }

            // Check if the player has enough of the currency
            if (currencyAmounts[currencyId] >= amount)
            {
                ChangeCurrency(currencyId, -amount);
                               

                return true;
            }

            CurrencyInfo currencyInfo = GetCurrencyInfo(currencyId);

            if (currencyInfo.showNotEnoughPopup && PopupManager.Exists())
            {
                object[] popupData =
                {
                    currencyInfo.id,
                    currencyInfo.popupTitleText,
                    currencyInfo.popupMessageText,
                    currencyInfo.popupHasStoreButton,
                    currencyInfo.popupHasRewardAdButton,
                    currencyInfo.rewardButtonText,
                    currencyInfo.rewardAmount
                };

                SoundManager.Instance.Play("btn-click");
                PopupManager.Instance.Show("store");
                
                if (ScreenManager.Instance.CurrentScreenId == "game")
                {
                    GameManager.Instance.IsPaused = true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gives the amount of currency
        /// </summary>
        public void Give(string currencyId, int amount)
        {
            if (!CheckCurrencyExists(currencyId))
            {
                return;
            }

            ChangeCurrency(currencyId, amount);
        }

        public void Give(string currencyId, int amount, float animDelay,bool isAnim)
        {
            if (!CheckCurrencyExists(currencyId))
            {
                return;
            }

            ChangeCurrency(currencyId, amount,animDelay,isAnim);
        }
        /// <summary>
        /// Gives the amount of currency, data is of the following format: "id;amount"
        /// </summary>
        public void Give(string data)
        {
            string[] stringObjs = data.Trim().Split(';');

            if (stringObjs.Length != 2)
            {
                Debug.LogErrorFormat("[CurrencyManager] Give(string data) : Data format incorrect: \"{0}\", should be \"id;amount\"", data);
                return;
            }

            string currencyId = stringObjs[0];
            string amountStr = stringObjs[1];

            int amount;

            if (!int.TryParse(amountStr, out amount))
            {
                Debug.LogErrorFormat("[CurrencyManager] Give(string data) : Amount must be an integer, given data: \"{0}\"", data);
                return;
            }

            if (!CheckCurrencyExists(currencyId))
            {
                return;
            }

            ChangeCurrency(currencyId, amount);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Sets the currency
        /// </summary>
        private void ChangeCurrency(string currencyId, int amount)
        {
            if (currencyId == "coins" && amount < 0)
                DailyMissionController.instance.MissionProgressUpdate(Mathf.Abs(amount), DailyMissionTypes.Coins);

            currencyAmounts[currencyId] += amount;

            if (currencyId == "coins")
            {
                UpdateCoinCurrencyTexts(currencyAmounts["coins"]);
            }
            if (OnCurrencyChanged != null)
            {
                OnCurrencyChanged(currencyId);
            }
        }

        private void ChangeCurrency(string currencyId, int amount,float animDelay = 0.2f,bool isAnim = false)
        {
            if (currencyId == "coins" && amount < 0)
                DailyMissionController.instance.MissionProgressUpdate(Mathf.Abs(amount), DailyMissionTypes.Coins);

            currencyAmounts[currencyId] += amount;

            if (currencyId == "coins")
            {
                StartCoroutine(GradualIncrementCoins(amount,animDelay));
                if (isAnim)
                {
                    StartCoroutine(PlayCashAnim(startObj,endObj));
                }
            }
           
        }

        public void UpdateCoinCurrencyTexts(int amount)
        {
            for (int i = 0; i < coinTexts.Count; ++i)
            {
                coinTexts[i].text = amount.ToString();
            }
        }
        public void UpdateCoinCurrencyTexts(float amount)
        {
            for (int i = 0; i < coinTexts.Count; ++i)
            {
                coinTexts[i].text = (int.Parse(coinTexts[i].text) + (int)amount).ToString();
            }
        }

        IEnumerator GradualIncrementCoins(int coins,float animDelay)
        {
            yield return new WaitForSeconds(animDelay);
            float inc = coins / 7f;
            float t = 0;

            while (t < 1f)
            {
                UpdateCoinCurrencyTexts(inc);
                //CurrencyManager.Instance.Give("coins", 1);
                t += 0.2f;
                yield return new WaitForSeconds(0.1f);
            }

            UpdateCoinCurrencyTexts(currencyAmounts["coins"]);
        }

        public void SetAnimObjs(GameObject start, GameObject end)
        {
            startObj = start;
            endObj   = end;
        }

        public void SetAnimObjsGamePlay(GameObject start)
        {
            startObj = start;
            endObj = gamePlayCoinsEndPoint;
        }

        // coin animation
        IEnumerator PlayCashAnim(GameObject parent, GameObject target)
        {
            SoundManager.Instance.Play("reward");
            float t = 0f;
            while (t < 1f)
            {
                yield return new WaitForSeconds(0.05f);
                StartCoroutine(CollectItemAnim(parent, target));
                t += 0.1f;
                //Debug.Log("t - "+t);
            }
            t = 0f;
            yield return new WaitForSeconds(1f);
        }

        private IEnumerator CollectItemAnim(GameObject parent, GameObject target)
        {
            GameObject cash = Instantiate(coinPref, parent.transform.position/*+ new Vector3(UnityEngine.Random.RandomRange(0, 1), UnityEngine.Random.Range(0,2), 0)*/, Quaternion.identity, parent.transform);
            //Debug.Log("cash  - " +cash.transform.position);
            cash.transform.DOMove(parent.transform.position + new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)), 0.3f);
            yield return new WaitForSeconds(0.3f);
            cash.transform.DOMove(target.transform.position, 1.2f).SetEase(Ease.InOutSine, 2f);
            yield return new WaitForSeconds(0.9f);
            target.transform.DOShakeScale(0.1f, 0.05f);
            Destroy(cash);
        }


        /// <summary>
        /// Sets all the starting currency amounts
        /// </summary>
        private void SetStartingValues()
        {
            for (int i = 0; i < currencyInfos.Count; i++)
            {
                CurrencyInfo currencyInfo = currencyInfos[i];

                currencyAmounts[currencyInfo.id] = currencyInfo.startingAmount;
            }
        }

        /// <summary>
        /// Gets the CUrrencyInfo for the given id
        /// </summary>
        private CurrencyInfo GetCurrencyInfo(string currencyId)
        {
            for (int i = 0; i < currencyInfos.Count; i++)
            {
                CurrencyInfo currencyInfo = currencyInfos[i];

                if (currencyId == currencyInfo.id)
                {
                    return currencyInfo;
                }
            }

            return null;
        }

        /// <summary>
        /// Checks that the currency exists
        /// </summary>
        private bool CheckCurrencyExists(string currencyId)
        {
            CurrencyInfo currencyInfo = GetCurrencyInfo(currencyId);

            if (currencyInfo == null || !currencyAmounts.ContainsKey(currencyId))
            {
                Debug.LogErrorFormat("[CurrencyManager] TrySpend : The given currencyId \"{0}\" does not exist", currencyId);

                return false;
            }

            return true;
        }

        #endregion

        #region Save Methods

        public Dictionary<string, object> Save()
        {
            Dictionary<string, object> saveData = new Dictionary<string, object>();

            saveData["amounts"] = currencyAmounts;

            return saveData;
        }

        public bool LoadSave()
        {
            JSONNode json = SaveManager.Instance.LoadSave(this);

            if (json == null)
            {
                return false;
            }

            foreach (KeyValuePair<string, JSONNode> pair in json["amounts"])
            {
                // Make sure the currency still exists
                if (GetCurrencyInfo(pair.Key) != null)
                {
                    currencyAmounts[pair.Key] = pair.Value.AsInt;
                }
            }

            return true;
        }

        #endregion
    }
}
