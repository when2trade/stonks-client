using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlotPoint : Clickable
{
    public string symbol;
    public bool canvasOpen = false;

    public void SetupPoint(Transform head, string symbol){
        foreach(SimpleTransformInfluenced component in GetComponentsInChildren<SimpleTransformInfluenced>()){
            component.referenceTransform = head;
        }
        GetComponentInChildren<TextMeshPro>().text = symbol;
        this.symbol = symbol;
        gameObject.name = symbol;
    }

    float[] stockData;
    GameObject panelForThis;

    public override void Click(Vector3 hitpos){
        if(canvasOpen) InfoPanelController.singleton.ClosePointPanel(this);
        else{
            panelForThis = InfoPanelController.singleton.OpenPointPanel(this);
            RelationalScatterPlotter.singleton.ShowEdgesConnectedTo(symbol);

            if(stockData == null){
                panelForThis.GetComponentInChildren<StockGraphPlotter>().HidePlot();
                ServerFetch.singleton.GetStockData(symbol, LoadStockData);
            }
            else
                ShowStockData();

        }
        canvasOpen = !canvasOpen;
    }

    void LoadStockData(DataCandle candle){
        stockData = new float[candle.c.Length];
        for(int i=0; i<stockData.Length; i++){
            stockData[i] = (candle.o[i] + candle.c[i])/2;
        }
        ShowStockData();
    }

    void ShowStockData(){
        panelForThis.GetComponentInChildren<StockGraphPlotter>().ShowPlot(stockData);
    }
}
