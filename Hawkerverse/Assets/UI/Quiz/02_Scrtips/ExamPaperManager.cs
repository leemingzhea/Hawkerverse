using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lordmin
{
    public class ExamPaperManager : MonoSingleton<ExamPaperManager>
    {


        [Header("[Set a perfect score]")]
        public float MaxScore = 100;

        [Header("[Set answer button color]")]
        public Color NormalAnswer_ButtonColor = new Color(223 / 255f, 245 / 255f, 255 / 255f);
        public Color SelectAnswer_ButtonColor = new Color(255 / 255f, 122 / 255f, 122 / 255f);
        public Color RightAnswer_ButtonColor = new Color(0, 0, 1);
        public Color WrongAnswer_ButtonColor = new Color(1, 0, 0);

        [Header("[Set Background Music]")]
        public AudioClip BGM_Main;
        [Range(0.1f, 3.0f)]
        public float Volume_BGM_Main = 1;
        public AudioClip BGM_ExamPaper;
        [Range(0.1f, 3.0f)]
        public float Volume_BGM_ExamPaper = 1;

        [Header("[Set Sounds]")]
        public AudioClip Sound_NormalSelect;
        [Range(0.1f, 3.0f)]
        public float Volume_NormalSelect = 1;
        public AudioClip Sound_SpecialSelect;
        [Range(0.1f, 3.0f)]
        public float Volume_SpecialSelect = 1;
        public AudioClip Sound_EndAnswerSelect;
        [Range(0.1f, 3.0f)]
        public float Volume_EndAnswerSelect = 1;

        private AudioSource AS;

        [HideInInspector]
        public GameObject ExamThemasParent;
        [HideInInspector]
        public GameObject[] ExamThemas;
        [HideInInspector]
        public int ThemaNumber_ExamThemas = 0;
        [HideInInspector]
        public GameObject ExamPaperEnd, ExamPaperStart, Button_Next, Button_Previous, Button_Settings;
        [HideInInspector]
        public float CurrentScore = 0;
        [HideInInspector]
        public bool IsEnd, IsNextAnimation = false;
        [HideInInspector]
        public int MaxQuestionCount, CurrentQuestionNumber, RightAnswerCount, WrongAnswerCount = 0;
        [HideInInspector]
        public Slider Slider_Question;
        [HideInInspector]
        public List<GameObject> List_Question = new List<GameObject>();
        [HideInInspector]
        public Text Text_QuestionThemaName, Text_QuestionNumber, Text_QuestionCount, Text_RightAnswer, Text_WrongAnswer, Text_Score, Text_MainTitle;


        // Use this for initialization
        void Start()
        {
            ExamThemas = new GameObject[ExamThemasParent.transform.childCount];

            if (GetComponent<AudioSource>())
                AS = GetComponent<AudioSource>();

            BGMPlay(BGM_Main, Volume_BGM_Main);

            Init();
        }

        private void Init()
        {
            CurrentQuestionNumber = 0;
            CurrentScore = 0;
            RightAnswerCount = 0;
            WrongAnswerCount = 0;

            for (int i = 0; i < ExamThemasParent.transform.childCount; i++)
            {
                ExamThemas[i] = ExamThemasParent.transform.GetChild(i).gameObject;
                ExamThemas[i].SetActive(false);
            }
            MaxQuestionCount = ExamThemas[ThemaNumber_ExamThemas].transform.childCount;
            ExamThemas[ThemaNumber_ExamThemas].SetActive(true);

            Text_MainTitle.text = ExamThemas[ThemaNumber_ExamThemas].name;


            List_Question.Clear();

            for (int i = 0; i < MaxQuestionCount; i++)
            {
                List_Question.Add(ExamThemas[ThemaNumber_ExamThemas].transform.GetChild(i).gameObject);
                List_Question[i].SetActive(false);
            }

            List_Question[CurrentQuestionNumber].SetActive(true);

            //Reset Button Color 
            for (int i = 0; i < List_Question.Count; i++)
                List_Question[i].GetComponent<Questions>().InitResetButtonColor();

            ExamPaperEnd.SetActive(false);

            Slider_Question.minValue = 0;
            Slider_Question.maxValue = MaxQuestionCount;
            Slider_Question.value = 0;


            SettingEndText();
            Text_QuestionNumber.text = "Q1";
            Text_QuestionThemaName.text = ExamThemas[ThemaNumber_ExamThemas].name;

            SetThemaButton();

            ExamPaperStart.SetActive(true);
        }

        public void NextQuestion()
        {

            if (IsNextAnimation == false)
                if (MaxQuestionCount - 1 > CurrentQuestionNumber)
                {
                    //normal Questions
                    CurrentQuestionNumber++;
                    StartCoroutine(IE_NextAnimation(List_Question[CurrentQuestionNumber - 1], List_Question[CurrentQuestionNumber]));
                    Play(Sound_NormalSelect, Volume_NormalSelect);

                }
                else
                {
                    //In Last Question
                    CurrentQuestionNumber++;
                    StartCoroutine(IE_NextAnimation(List_Question[CurrentQuestionNumber - 1], ExamPaperEnd));
                    Button_EndQuestion();
                    AS.Stop();
                    Play(Sound_EndAnswerSelect, Volume_EndAnswerSelect);

                }
        }


        //Changed from Asset Version 1.1 to go beyond the last page to the resulting UI immediately.
        public IEnumerator IE_NextAnimation(GameObject previous, GameObject next, float time = 0.5f)
        {
            IsNextAnimation = true;
            next.SetActive(false);

            float t = 0;
            if (next != ExamPaperEnd)
            {
                //Not Last Paper
                while (t < time)
                {
                    t += Time.deltaTime;
                    previous.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(1f, 0f, t / time);
                    yield return null;
                }
            }

            previous.SetActive(false);
            next.SetActive(true);

            Slider_Question.value = CurrentQuestionNumber;


            if (Slider_Question.value != Slider_Question.maxValue)
            {
                Text_QuestionNumber.text = "Q" + (CurrentQuestionNumber + 1);
                Text_QuestionThemaName.text = ExamThemas[ThemaNumber_ExamThemas].name;
            }
            else
            {
                Text_QuestionNumber.text = "";
                Text_QuestionThemaName.text = "";
            }

            t = 0;
            while (t < time)
            {
                t += Time.deltaTime;
                next.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(0, 1f, t / time);
                yield return null;
            }
            IsNextAnimation = false;
        }

        public void SettingEndText()
        {
            Text_QuestionCount.text = "Total Questions: " + MaxQuestionCount;
            Text_RightAnswer.text = "Right Answer: " + RightAnswerCount;
            Text_WrongAnswer.text = "Wrong Answer: " + WrongAnswerCount;
            Text_Score.text = "Score: " + CurrentScore.ToString("##0.##") + " / " + MaxScore.ToString("##0.##");
            Text_QuestionNumber.text = "";
            Text_QuestionThemaName.text = "";

        }


        public void Button_EndQuestion()
        {

            IsEnd = true;

            //Setting Buttons: Right Answer / Wrong Answer
            for (int i = 0; i < List_Question.Count; i++)
                List_Question[i].GetComponent<Questions>().SettingButton_RightWrongColor();

            Text_QuestionCount.text = "Total Questions: " + MaxQuestionCount;
            Text_RightAnswer.text = "Right Answer: " + RightAnswerCount;
            Text_RightAnswer.color = RightAnswer_ButtonColor;
            Text_WrongAnswer.text = "Wrong Answer: " + WrongAnswerCount;
            Text_WrongAnswer.color = WrongAnswer_ButtonColor;
            Text_QuestionNumber.text = "";
            Text_QuestionThemaName.text = "";

            //score animation
            StartCoroutine(IE_ScoreAni(3));
        }

        IEnumerator IE_ScoreAni(float time)
        {
            Image image = Text_Score.transform.parent.GetComponent<Image>();
            Color startColor = Color.white;
            Color endColor = image.color;
            float t = 0;
            float tempScore = 0;
            Text_Score.text = "Score: " +  "0 / " + MaxScore.ToString("##0.##");
            while (t < time)
            {
                t += Time.deltaTime;
                tempScore = Mathf.Lerp(0, CurrentScore, t / time);
                image.color = Color.Lerp(startColor, endColor, t / time);
                Text_Score.text = "Score: " + tempScore.ToString("##0.##") + " / " + MaxScore.ToString("##0.##");
                yield return null;
            }
            Text_Score.text = "Score: " + CurrentScore.ToString("##0.##") + " / " + MaxScore.ToString("##0.##");

            t = 0;
            while (t < 0.2f)
            {
                t += Time.deltaTime;
                image.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 1.2f, t / 0.2f);
                yield return null;
            }

            t = 0;
            while (t < 0.5f)
            {
                t += Time.deltaTime;
                image.transform.localScale = Vector3.Lerp(Vector3.one * 1.2f, Vector3.one,  t / 0.5f);
                yield return null;
            }
        }

        public void Button_Main()
        {
            BGMPlay(BGM_Main, Volume_BGM_Main);
            Play(Sound_SpecialSelect, Volume_SpecialSelect);

            //Reset
            IsEnd = false;
            CurrentScore = 0;
            RightAnswerCount = 0;
            WrongAnswerCount = 0;
            SettingEndText();

            Text_QuestionNumber.GetComponent<Outline>().enabled = false;
            Text_QuestionNumber.GetComponent<Text>().text = "Q1";
            Text_QuestionThemaName.text = ExamThemas[ThemaNumber_ExamThemas].name;
            Text_QuestionNumber.GetComponent<Text>().color = Color.white;


            //Reset Button Color 
            for (int i = 0; i < List_Question.Count; i++)
                List_Question[i].GetComponent<Questions>().InitResetButtonColor();

            StartCoroutine(IE_NextAnimation(ExamPaperEnd, ExamPaperStart));

            Slider_Question.gameObject.SetActive(false);
        }

        public void Button_Retry()
        {
            BGMPlay(BGM_ExamPaper, Volume_BGM_ExamPaper);
            Play(Sound_SpecialSelect, Volume_SpecialSelect);

            //Reset
            IsEnd = false;
            CurrentScore = 0;
            RightAnswerCount = 0;
            WrongAnswerCount = 0;
            SettingEndText();

            Text_QuestionNumber.GetComponent<Outline>().enabled = false;
            Text_QuestionNumber.GetComponent<Text>().text = "Q1";
            Text_QuestionNumber.GetComponent<Text>().color = Color.white;
            Text_QuestionThemaName.text = ExamThemas[ThemaNumber_ExamThemas].name;


            //Reset Button Color 
            for (int i = 0; i < List_Question.Count; i++)
                List_Question[i].GetComponent<Questions>().InitResetButtonColor();

            //Retry
            Slider_Question.value = 0;
        }

        public void Button_Quit()
        {
            Play(Sound_SpecialSelect, Volume_SpecialSelect);

            Application.Quit();
        }


        public void Button_Start()
        {
            Play(Sound_SpecialSelect, Volume_SpecialSelect);

            Init();
            BGMPlay(BGM_ExamPaper, Volume_BGM_ExamPaper);
            Slider_Question.gameObject.SetActive(true);
            ExamThemasParent.SetActive(true);
            StartCoroutine(IE_NextAnimation(ExamPaperStart, List_Question[CurrentQuestionNumber]));
        }

        bool IsTextAnimation = false;
        IEnumerator TextAnimation(float time = 0.2f)
        {
            if (!IsTextAnimation)
            {
                IsTextAnimation = true;
                float t = 0;
                Vector3 startScale = Text_MainTitle.transform.localScale;
                Vector3 endScale = Text_MainTitle.transform.localScale * 1.2f;
                while (t < time)
                {
                    t += Time.deltaTime;
                    Text_MainTitle.transform.localScale = Vector3.Lerp(startScale, endScale, t / time);
                    yield return null;
                }
                t = 0;
                while (t < time)
                {
                    t += Time.deltaTime;
                    Text_MainTitle.transform.localScale = Vector3.Lerp(endScale, startScale, t / time);
                    yield return null;
                }
                IsTextAnimation = false;
                yield return null;
            }
            yield return null;
        }

        public void SetThemaButton()
        {

            Button_Next.gameObject.SetActive(true);
            Button_Previous.gameObject.SetActive(true);

            if (ExamThemas.Length == 1)
            {
                Button_Next.gameObject.SetActive(false);
                Button_Previous.gameObject.SetActive(false);
                return;
            }

            if (ThemaNumber_ExamThemas == ExamThemas.Length - 1)
            {
                Button_Next.gameObject.SetActive(false);
                return;
            }

            if (ThemaNumber_ExamThemas == 0)
            {
                Button_Previous.gameObject.SetActive(false);
                return;
            }
        }

        public void Button_NextThema()
        {
            Play(Sound_NormalSelect, Volume_NormalSelect);
            if (ThemaNumber_ExamThemas < ExamThemas.Length - 1)
            {
                ThemaNumber_ExamThemas++;
                Text_MainTitle.text = ExamThemas[ThemaNumber_ExamThemas].name;
            }
            else
            {
                ThemaNumber_ExamThemas = 0;
                Text_MainTitle.text = ExamThemas[ThemaNumber_ExamThemas].name;
            }

            StartCoroutine(TextAnimation());
            SetThemaButton();
        }

        public void Button_PreviousThema()
        {
            Play(Sound_NormalSelect, Volume_NormalSelect);
            if (ThemaNumber_ExamThemas > 0)
            {
                ThemaNumber_ExamThemas--;
                Text_MainTitle.text = ExamThemas[ThemaNumber_ExamThemas].name;
            }
            else
            {
                ThemaNumber_ExamThemas = ExamThemas.Length - 1;
                Text_MainTitle.text = ExamThemas[ThemaNumber_ExamThemas].name;
            }

            StartCoroutine(TextAnimation());
            SetThemaButton();
        }

        public void Button_SoundOnOff()
        {
            if (AS.mute)
            {
                AS.mute = false;
                Button_Settings.transform.GetChild(0).GetComponent<Text>().text = "Sound";
            }
            else
            {
                AS.mute = true;
                Button_Settings.transform.GetChild(0).GetComponent<Text>().text = "Mute";
            }
                

        }

        public void BGMPlay(AudioClip bgm, float volume = 1)
        {
            if (AS.isPlaying)
                AS.Stop();

            if (bgm)
                AS.clip = bgm;
            else
                AS.clip = null;

            AS.loop = true;
            AS.volume = volume;
            AS.Play();
        }

        public void Play(AudioClip clip, float volume = 1)
        {
            AS.PlayOneShot(clip, volume);
        }

    }
}

