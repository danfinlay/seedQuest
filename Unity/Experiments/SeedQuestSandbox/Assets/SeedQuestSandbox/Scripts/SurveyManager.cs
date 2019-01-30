﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MaterialUI;
using TMPro;

public class SurveyManager : MonoBehaviour
{
    public SurveyData data;
    public GameObject cardContainer;
    public GameObject dotContainer;
    public GameObject cardTemplateOpen;
    public GameObject cardTemplateScale;
    public GameObject cardTemplateSubmit;
    //public GameObject CardTemplateRank;
    public Button PreviousButton;
    public Button NextButton;

    public Sprite ring;
    public Sprite dot;

    private int xOffset = 0;
    private int imageXOffset = -34;
    private float cardContainerSize = 0;
    private int currentCardIndex = 0;

    private List<GameObject> dots = new List<GameObject>();

    private void Start()
    {
        int surveyQuestions = data.surveyData.Count;
        var cardContainerTransform = cardContainer.transform as RectTransform;
        //cardContainerSize = surveyQuestions * 4000f;
        //cardContainerTransform.sizeDelta = new Vector2(cardContainerSize, cardContainerTransform.sizeDelta.y);

        ColorBlock cb = PreviousButton.colors;
        cb.disabledColor = new Color(0, 0, 0, 0);
        cb = NextButton.colors;
        cb.disabledColor = new Color(0, 0, 0, 0);

        var dotContainerTransform = dotContainer.transform as RectTransform;
        //float dotContainerSize = 40 * surveyQuestions - 8;
        //dotContainerTransform.sizeDelta = new Vector2(dotContainerSize, dotContainerTransform.sizeDelta.y);
        for(int i = 0; i < surveyQuestions; i++)
        {
            if (data.surveyData[i].type == SurveyDataItem.QuestionType.Open)
            {

                var newCard = Instantiate(cardTemplateOpen);
                newCard.SetActive(true);
                newCard.transform.parent = cardContainer.transform;
                newCard.transform.localPosition = new Vector3(xOffset, 0, 0);
                xOffset += 4000;
                TMP_Text text = newCard.transform.GetChild(1).GetChild(1).GetComponent<TMP_Text>();
                text.text = data.surveyData[i].question;

                TMP_InputField input = newCard.transform.GetChild(1).GetChild(2).GetComponent<TMP_InputField>();
                int tempI = i;
                input.onValueChanged.AddListener(delegate
                {
                    data.surveyData[tempI].answer = input.text;
                });
                text = newCard.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>();
                text.text = (i + 1).ToString() + "/" + surveyQuestions;
            }

            else
            {
                var newCard = Instantiate(cardTemplateScale);
                newCard.SetActive(true);
                newCard.transform.parent = cardContainer.transform;
                newCard.transform.localPosition = new Vector3(xOffset, 0, 0);
                xOffset += 4000;
                TMP_Text text = newCard.transform.GetChild(1).GetChild(1).GetComponent<TMP_Text>();
                text.text = data.surveyData[i].question;

                data.surveyData[i].answers = new string[data.surveyData[i].questions.Length];

                text = newCard.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>();
                text.text = (i + 1).ToString() + "/" + surveyQuestions;

                var column = newCard.transform.GetChild(1).GetChild(4).GetChild(1).GetChild(0).GetComponent<TMP_Text>();
                var row = newCard.transform.GetChild(1).GetChild(4).GetChild(0).GetChild(0).gameObject;
                List<GameObject> rows = new List<GameObject>();
                List<TMP_Text> columns = new List<TMP_Text>();
                float columnXOffset = -112f;
                float columnWidth = 436f / (float)data.surveyData[i].headers.Length;
                float rowYOffset = 28f;
                float rowHeight = 245f / (float)data.surveyData[i].questions.Length;

                foreach (string header in data.surveyData[i].headers)
                {
                    TMP_Text newColumn = Instantiate(column);
                    newColumn.transform.parent = column.transform.parent;
                    newColumn.transform.localPosition = new Vector3(columnXOffset, 66, 0);
                    newColumn.transform.localScale = new Vector3(1, 1, 1);
                    newColumn.GetComponent<RectTransform>().sizeDelta = new Vector2(columnWidth, 32);
                    newColumn.text = header;
                    columns.Add(newColumn);

                    columnXOffset += columnWidth;              
                }

                Destroy(column.gameObject);

                foreach (string header in data.surveyData[i].questions)
                {
                    GameObject newRow = Instantiate(row);
                    newRow.transform.parent = row.transform.parent;
                    newRow.transform.localPosition = new Vector3(0, 0, 0);
                    newRow.transform.localScale = new Vector3(1, 1, 1);
                    rows.Add(newRow);
                }

                // I tried to include this in the above loop, but position data wasn't being saved for some reason.
                for (int j = 0; j < rows.Count; j++)
                {
                    TMP_Text question = rows[j].transform.GetChild(0).GetComponent<TMP_Text>();
                    question.transform.localPosition = new Vector3(-218, rowYOffset, 0);
                    question.transform.localScale = new Vector3(1, 1, 1);
                    question.GetComponent<RectTransform>().sizeDelta = new Vector2(136, rowHeight);
                    question.text = data.surveyData[i].questions[j];

                    for(int k = 0; k < columns.Count; k++)
                    {
                        Toggle newToggle = Instantiate(rows[j].transform.GetChild(1).GetChild(0).GetComponent<Toggle>());
                        newToggle.transform.parent = rows[j].transform.GetChild(1).GetChild(0).parent;
                        newToggle.transform.localPosition = new Vector3(columns[k].transform.localPosition.x, rowYOffset, 0);
                        newToggle.transform.localScale = new Vector3(1, 1, 1);
                        newToggle.name = columns[k].text;
                        int tempI = i;
                        int tempJ = j;
                        newToggle.onValueChanged.AddListener(delegate {
                            if (newToggle.isOn)
                            {
                                data.surveyData[tempI].answers[tempJ] = newToggle.name;
                            }
                        });
                        newToggle.group = rows[j].transform.GetChild(1).gameObject.GetComponent<ToggleGroup>();
                    }
                    Destroy(rows[j].transform.GetChild(1).GetChild(0).gameObject);
                    rows[j].transform.GetChild(1).gameObject.GetComponent<ToggleGroup>().SetAllTogglesOff();
                    rowYOffset -= rowHeight;

                }

                Destroy(row);
            }

            /*else if (data.surveyData[i].type == SurveyDataItem.QuestionType.Scale)
            {
                var newCard = Instantiate(CardTemplateScale);
                newCard.SetActive(true);
                newCard.transform.parent = cardContainer.transform;
                newCard.transform.localPosition = new Vector3(xOffset, 0, 0);
                xOffset += 4000;
                Text text = newCard.transform.GetChild(1).GetChild(1).GetComponent<Text>();
                text.text = data.surveyData[i].question;

                text = newCard.transform.GetChild(1).GetChild(0).GetComponent<Text>();
                text.text = (i + 1).ToString() + "/" + surveyQuestions;
                Slider slider = newCard.transform.GetChild(1).GetChild(2).GetChild(0).GetComponent<Slider>();
                slider.minValue = data.surveyData[i].scaleStart;
                slider.maxValue = data.surveyData[i].scaleStop;
                slider.value = data.surveyData[i].scaleDefault;
            }

            else
            {
                int dropdownXOffset = 0;
                int numberText = 2;
                var newCard = Instantiate(CardTemplateRank);
                newCard.SetActive(true);
                newCard.transform.parent = cardContainer.transform;
                newCard.transform.localPosition = new Vector3(xOffset, 0, 0);
                xOffset += 4000;
                Text text = newCard.transform.GetChild(1).GetChild(1).GetComponent<Text>();
                text.text = data.surveyData[i].question;

                text = newCard.transform.GetChild(1).GetChild(0).GetComponent<Text>();
                text.text = (i + 1).ToString() + "/" + surveyQuestions;

                TMP_Text number = newCard.transform.GetChild(1).GetChild(2).GetChild(0).GetComponent<TMP_Text>();
                TMP_Dropdown dropdown = newCard.transform.GetChild(1).GetChild(2).GetChild(1).GetComponent<TMP_Dropdown>();

                List<TMP_Dropdown> dropdownList = new List<TMP_Dropdown>();
                dropdownList.Add(dropdown);
                List<TMP_Dropdown.OptionData> optionList = new List<TMP_Dropdown.OptionData>();

                foreach (string rank in data.surveyData[i].ranks)
                {
                    TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(rank, null);
                    optionList.Add(option);

                    if(optionList.Count > dropdownList.Count)
                    {

                        TMP_Text newNumber = Instantiate(number);
                        newNumber.transform.parent = number.transform.parent;
                        newNumber.transform.localPosition = new Vector3(-260, dropdownXOffset, 0);
                        newNumber.transform.localScale = new Vector3(1, 1, 1);
                        newNumber.text = numberText.ToString() + ".";

                        TMP_Dropdown newDropdown = Instantiate(dropdown);
                        newDropdown.transform.parent = dropdown.transform.parent;
                        newDropdown.transform.localPosition = new Vector3(32, dropdownXOffset, 0);
                        newDropdown.transform.localScale = new Vector3(1, 1, 1);
                        dropdownList.Add(newDropdown);

                        dropdownXOffset -= 80;
                        numberText += 1;
                    }
                }

                foreach (TMP_Dropdown item in dropdownList)
                {
                    foreach(TMP_Dropdown.OptionData option in optionList)
                    {
                        item.options.Add(option);
                    }
                }


            }*/

            GameObject newDot = new GameObject();
            RectTransform dotTransform = newDot.AddComponent<RectTransform>();
            dotTransform.sizeDelta = new Vector2(32, 32);
            dotTransform.anchorMin = new Vector2(0, 0.5f);
            dotTransform.anchorMax = new Vector2(0, 0.5f);
            Image newImage = newDot.AddComponent<Image>();
            if (i == currentCardIndex)
            {
                newImage.sprite = dot;
            }
            else
            {
                newImage.sprite = ring;
            }
            newDot.transform.parent = dotContainer.transform;
            newDot.transform.localPosition = new Vector3(imageXOffset, 0, 0);
            imageXOffset += 40;
            dots.Add(newDot);
        }

        GameObject submitDot = new GameObject();
        RectTransform submitDotTransform = submitDot.AddComponent<RectTransform>();
        submitDotTransform.sizeDelta = new Vector2(32, 32);
        submitDotTransform.anchorMin = new Vector2(0, 0.5f);
        submitDotTransform.anchorMax = new Vector2(0, 0.5f);
        Image submitDotImage = submitDot.AddComponent<Image>();
        submitDotImage.sprite = ring;
        submitDot.transform.parent = dotContainer.transform;
        submitDot.transform.localPosition = new Vector3(imageXOffset, 0, 0);
        dots.Add(submitDot);

        var submitCard = Instantiate(cardTemplateSubmit);
        submitCard.SetActive(true);
        submitCard.transform.parent = cardContainer.transform;
        submitCard.transform.localPosition = new Vector3(xOffset, 0, 0);
        Button submitButton = submitCard.transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<Button>();
        submitButton.onClick.AddListener(delegate
        {
            Debug.Log("Submit Button Clicked");
            Application.Quit();
        });

        PreviousButton.onClick.AddListener(onClickPrevious);
        NextButton.onClick.AddListener(onClickNext);
    }

    public void onClickPrevious()
    {
        if (currentCardIndex > 0)
        {
            Vector3 destination = cardContainer.transform.position + new Vector3(4000, 0, 0);
            StartCoroutine(MoveOverSeconds(cardContainer, destination, 1));

            dots[currentCardIndex].GetComponent<Image>().sprite = ring;
            currentCardIndex -= 1;
            dots[currentCardIndex].GetComponent<Image>().sprite = dot;
        }
    }

    public void onClickNext()
    {
        if (currentCardIndex < data.surveyData.Count)
        {
            Vector3 destination = cardContainer.transform.position - new Vector3(4000, 0, 0);
            StartCoroutine(MoveOverSeconds(cardContainer, destination, 1));


            dots[currentCardIndex].GetComponent<Image>().sprite = ring;
            currentCardIndex += 1;
            dots[currentCardIndex].GetComponent<Image>().sprite = dot;
        }
    }

    private IEnumerator MoveOverSeconds(GameObject objectToMove, Vector3 end, float seconds)
    {
        PreviousButton.enabled = false;
        NextButton.enabled = false;
        float elapsedTime = 0;
        Vector3 startingPos = objectToMove.transform.position;
        while (elapsedTime < seconds)
        {
            objectToMove.transform.position = Vector3.Lerp(startingPos, end, (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        objectToMove.transform.position = end;
        PreviousButton.enabled = true;
        NextButton.enabled = true;
    }
}