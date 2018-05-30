using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TestGame
{
    public class SolarGpp : MonoBehaviour
    {

        // UI 
        public Button               m_SpawnButton;
        private Text                m_SpawnButtonText;
        private string              m_SpawnButtonTextString;

        public Button               m_LangButton;
        private Text                m_LangButtonText;
        private string              m_LangButtonTextString;

        public Slider               m_SpeedSlider;

        private List<GameObject>    m_plantLists = new List<GameObject>(9);

        public List<string>         m_plantNames = new List<string>(8);

        virtual public void OnDestroy()
        {
            Localization.Instance.OnChangeLocalization -= OnChangeLocal;
        }

        private void Start()
        {
            m_SpawnButtonText = m_SpawnButton.GetComponentInChildren<Text>();
            m_SpawnButtonTextString = m_SpawnButtonText.text;

            m_LangButtonText = m_LangButton.GetComponentInChildren<Text>();
            m_LangButtonTextString = m_LangButtonText.text;

            Localization.Instance.OnChangeLocalization += OnChangeLocal;
            Localization.Instance.Active();
        }

        void OnChangeLocal()
        {
            m_SpawnButtonText.text = Localization.Instance.GetText(m_SpawnButtonTextString);
            m_LangButtonText.text = Localization.Instance.GetText(m_LangButtonTextString);
        }

        public void SpawnSolar()
        {
            m_SpawnButton.gameObject.SetActive(false);
            m_SpeedSlider.gameObject.SetActive(true);
            m_SpeedSlider.value = Time.timeScale;
            StartCoroutine(SpawnPlanet());
        }

        public void AdjustSpeed()
        {
            Time.timeScale = m_SpeedSlider.value;
        }

        IEnumerator SpawnPlanet()
        {
            for (int nIndex = 0; nIndex < m_plantNames.Count; ++nIndex)
            {
                string name = m_plantNames[nIndex];
                string[] subname = name.Split(';');
                GameObject parent = null;
                for( int subIndex = 0; subIndex < subname.Length; ++ subIndex )
                {
                    string planetName = subname[subIndex];
                    TestObject to = null;
                    for( int i = 0; i < ObjectLists.Instance.objectList.Count; ++i )
                    {
                       if (  string.Compare(planetName , ObjectLists.Instance.objectList[i].name ) == 0 )
                        {
                            to = ObjectLists.Instance.objectList[i];
                            break;
                        }
                    }
                    if ( to != null )
                    {
                        GameObject planet = Utilities.InstantiateTestObject(to);
                        if ( planet != null )
                        {
                            m_plantLists.Add(planet);
                            if ( subIndex == 0 )
                                parent = planet;
                            else if ( parent != null )
                                planet.transform.parent = parent.transform;
                        }
                    }
                }
                yield return new WaitForSeconds(2);
            }
            yield break;
        }


        private void OnGUI()
        {
            // simple show object names
            for (int i = 0; i < m_plantLists.Count; ++i)
            {
                float dis = Vector3.SqrMagnitude(m_plantLists[i].transform.position - Camera.main.transform.position);
                if (dis < 100.0f)
                {
                    Vector3 screenpos = Camera.main.WorldToScreenPoint(m_plantLists[i].transform.position);
                    if ( screenpos.z > 0 )
                    GUI.TextArea(new Rect(screenpos.x, Screen.height - screenpos.y, 100, 20),  Localization.Instance.GetText( m_plantLists[i].name) );
                }
            }
        }

        public void OnChangeLang()
        {
            int count = Localization.Instance.Langs.Count;
            int index = 0;
            for( int i = 0; i < count; ++i )
            {
                if ( string.Compare( Localization.Instance.Language, Localization.Instance.Langs[i] ) == 0 )
                {
                    index = i + 1;
                    break;
                }
            }
            if ( index >= count )
                index = 0;
            Localization.Instance.Language = Localization.Instance.Langs[index];
        }
    }
}
