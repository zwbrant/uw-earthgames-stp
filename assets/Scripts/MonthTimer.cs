using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class MonthTimer : MonoBehaviour {
    internal static PlayerStatus player;
	public static bool pause = false;

	public Slider timeMeter; //UI timer meter
	internal static int monthIndex;
	private static int year = 2034;


	public static float incomeDue; 
	public float monthLength; 
	public Dictionary<string, MonthData> history = new Dictionary<string, MonthData>();

    public static string[] months = new string[] {
        "Jan", "Feb", "March", "April",
        "May", "June", "July", "Aug", "Sept",
        "Oct", "Nov", "Dec"
    };
    public static int[] monthLengths = new int[] 
    { 31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

    internal int currDay = 1;
    internal int currMonth = 1;
    internal float dayTimer; 

	public GameObject progReport;
    private GameObject currReport;
	public Text monthText;

    public RotateSun _rotateSun;
    public float _monthLength;
    public float _currTime;
    public GameObject historyGraph;
    public GameObject pieGraph;

	// Use this for initialization
	void Awake () {
        player = GameObject.Find("Manager").GetComponent<PlayerStatus>();
	}

    // Use this for initialization
    void Start()
    {
        monthIndex = 0;
        year = 2016;
        SetDateText();

        // _currTime = (180f / _rotateSun.startRotation) * _monthLength;
    }

    // Update is called once per frame
    void Update () {
		//if (timer == monthLength) {
		//	Month.GetComponent<Text>().text = months[monthIndex] + " " + year;
		//}

		//if(timer > 0){
		//	if (!pause) {
		//		timeMeter.value -= Time.deltaTime; 
		//		timer -= Time.deltaTime;
		//	}
		//}
		//else {
		//	MonthData curr = new MonthData(PlayerStatus.carbonSaved, PlayerStatus.incomeAdded, PlayerStatus.moneySpent);
		//	history.Add(months[monthIndex] + year.ToString(), curr); 
		//	ProduceReport();
		//	PlayerStatus.incomeAdded = 0;
		//	PlayerStatus.carbonSaved = 0;
		//	PlayerStatus.moneySpent = 0;

		//	monthIndex = (monthIndex < 11) ? monthIndex + 1 : 0;
		//	year = (monthIndex == 0) ? year + 1 : year;

		//	timer = monthLength;
		//	timeMeter.value = monthLength;
		//}
	}

    private List<Vector2> carbHistory = new List<Vector2>() { new Vector2(1, 0), new Vector2(2, 0) };
    private List<Vector2> incomeHistory = new List<Vector2>() { new Vector2(1, 0), new Vector2(2, 0) };

    public void EndOfMonth()
    {
        PlayerStatus.money += PlayerStatus.income;
        player.PauseGame(true, true);
        MonthData curr = new MonthData(PlayerStatus.carbonSaved, PlayerStatus.incomeAdded, PlayerStatus.moneySpent);
        history.Add(months[monthIndex] + ", " + year.ToString(), curr);
        carbHistory.Add(new Vector2(monthIndex + 1, 
            carbHistory[carbHistory.Count - 1].y + PlayerStatus.carbonSaved));
        incomeHistory.Add(new Vector2(monthIndex + 1,
            incomeHistory[incomeHistory.Count - 1].y + PlayerStatus.incomeAdded));
        ProduceReport();
        PlayerStatus.incomeAdded = 0;
        PlayerStatus.carbonSaved = 0;
        PlayerStatus.moneySpent = 0;

        currDay = 1;
        monthIndex = (monthIndex < 11) ? monthIndex + 1 : 0;
        year = (monthIndex == 0) ? year + 1 : year;

        SetDateText();
    }

    public void SetDateText()
    {
        monthText.text = months[monthIndex] + " " + currDay + ", " + year;
    }

 //   public void StartTimer() {
	//	timeMeter.maxValue = monthLength;
	//	monthIndex = 0;
	//}

	public void ProduceReport() {
		currReport = (GameObject)Instantiate (progReport);
        currReport.transform.SetParent(GameObject.Find("UI").transform, false);
        //report.GetComponent<RectTransform>().localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        currReport.transform.GetChild(0).GetComponent<Text>().text = "Monthly Report: " + months[monthIndex] + ", " + year.ToString();

        int greenHouses = 0;
        int yellowHouses = 0;
        int redHouses = 0;
        float carbEfficSum = 0f;

        foreach (float house in StatsAndTriggers.carbEfficiencies.Values) 
        {
            if (!float.IsNaN(house))
                carbEfficSum += house;
            if (house < 25f)
                redHouses++;
            else if (house > 75f)
                greenHouses++;
            else
                yellowHouses++;
        }


        int total = StatsAndTriggers.carbEfficiencies.Count;
        currReport.transform.GetChild(1).GetComponent<Text>().text = greenHouses + "/" +  total;
        currReport.transform.GetChild(2).GetComponent<Text>().text = yellowHouses + "/" + total;
        currReport.transform.GetChild(3).GetComponent<Text>().text = redHouses + "/" + total;
        currReport.transform.GetChild(4).GetComponent<Text>().text = Mathf.Round(carbEfficSum / total) + "%";

        historyGraph = currReport.transform.GetChild(5).gameObject;
        pieGraph = currReport.transform.GetChild(6).gameObject;
        historyGraph.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => this.SwitchGraph());
        BuildPieGraph();
        SetHistGraphSeries(carbHistory);

    }


    public WMG_Series series;
    int histGraphY = 20;

    private void SetHistGraphSeries(List<Vector2> history)
    {
        WMG_Axis_Graph graph = historyGraph.GetComponent<WMG_Axis_Graph>();
        if (series != null)
            series.pointValues.Clear();
        else 
            series = graph.addSeries();
       
        //Set the month labels (x axis)
        SetMonthLabels(graph);

        List<Vector2> ptValues;
        if (history.Count >= 4)
        {//IF there's enough history to fill the graph, get the last 3 values
            ptValues = history.GetRange(carbHistory.Count - 3, 3);
        }
        else {//Else, just use what there is
            ptValues = history;
        }

        histGraphY = 10;
        for (int i = 0; i < ptValues.Count; i++) {
            if (histGraphY < ptValues[i].y)
                histGraphY = (int)ptValues[i].y + 100;

            ptValues[i] = new Vector2(i + 1, ptValues[i].y);
        }

        graph.yAxis.AxisMaxValue = histGraphY;
        Color ptColor = new Color(134f/255f, 115f/255f, 63f/255f);
        series.pointColor = ptColor;
        series.pointValues.SetList(ptValues);
    }

    private void BuildPieGraph()
    {
        WMG_Pie_Graph graph = pieGraph.GetComponent<WMG_Pie_Graph>();
        graph.sliceValues = new WMG_List<float>() { 1f, 3f, 2f };
        Debug.Log(graph.sliceValues[1]);
    }

    internal void SetMonthLabels(WMG_Axis_Graph graph)
    {
        List<string> monthLabels;
        if (monthIndex == 0)
            monthLabels = new List<string> { months[10], months[11], months[0], months[1] };
        else if (monthIndex == 1)
            monthLabels = new List<string> { months[11], months[0], months[1], months[2] };
        else if (monthIndex == 11)
            monthLabels = new List<string> { months[monthIndex - 2], months[monthIndex - 1],
                months[monthIndex], months[0] };
        else
            monthLabels = new List<string> { months[monthIndex - 2], months[monthIndex - 1],
                    months[monthIndex], months[monthIndex + 1] };
        graph.xAxis.axisLabels.SetList(monthLabels);
    }

    bool isCarbGraph = true;

    public void SwitchGraph()
    {
        Debug.Log("Switchin Graph");
        isCarbGraph = !isCarbGraph;

        Text graphTitle = historyGraph.transform.GetChild(3).GetComponent<Text>();

        if (isCarbGraph)
        {
            graphTitle.text = "Co2 Savings";
            SetHistGraphSeries(carbHistory);
        }
        else {
            graphTitle.text = "Power Bill Savings";
            SetHistGraphSeries(incomeHistory);
        }
    }
}
