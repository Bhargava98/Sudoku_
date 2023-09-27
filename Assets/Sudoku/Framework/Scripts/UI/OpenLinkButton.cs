using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BizzyBeeGames
{
	[RequireComponent(typeof(Button))]
	public class OpenLinkButton : MonoBehaviour
	{
		#region Inspector Variables

		[SerializeField] private string url = "";

		#endregion

		#region Unity Methods

		private void Start()
		{
			gameObject.GetComponent<Button>().onClick.AddListener(() =>  OpenLink() );
		}

        private void OpenLink()
        {
            SoundManager.Instance.Play("btn-click");
            if (PopupManager.Instance.NoInternetNativePopup("Network", "No Internet Access") == false)
            {
                Invoke("GotoLink",0.2f);
            }
        }

        void GotoLink()
        {
            Application.OpenURL(url);
        }
		#endregion
	}
}
