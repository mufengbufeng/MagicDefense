using System.Collections;
using System.Collections.Generic;
using GameConfig;
using TEngine;
using TMPro;
using UnityEngine;

namespace GameLogic
{
    public class LanguageComponent : MonoBehaviour
    {
        public int LanguageId;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake()
        {
            if (LanguageId == 0)
            {
                return;
            }

            TextMeshProUGUI textMeshProUGUI = transform.GetComponent<TextMeshProUGUI>();
            if (textMeshProUGUI != null)
            {
                textMeshProUGUI.text = ConfigSystem.Instance.Tables.LanguageConfig.GetOrDefault(LanguageId).Chinese;
            }
        }
    }
}
