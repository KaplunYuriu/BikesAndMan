// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open FSharp.Data;
open FSharp.Charting;
open System;

module public DayDataSet = 
    type CsvData = CsvProvider<"C:/code/BikesAndMen/BikesAndMen/Dataset/day.csv">
    let data = CsvData.Load("C:/code/BikesAndMen/BikesAndMen/Dataset/day.csv").Rows
    


[<EntryPoint>]
let main argv = 
    let all = Chart.Line([ for obs in DayDataSet.data -> obs.Cnt ]).ShowChart()
    System.Windows.Forms.Application.Run(all)
    let value = Console.ReadLine()
    0 // return an integer exit code
