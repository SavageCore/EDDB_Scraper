﻿Imports System.Linq
Imports System.Text.RegularExpressions
Imports HtmlAgilityPack

Public Class EDDB_Scraper

    Public Shared Function VA_DisplayName() As String
        Return "EDDB Loop Scraper"
    End Function

    Public Shared Function VA_Id() As Guid
        Return New Guid("{CA49A0E3-8ECA-45CC-B8EF-B309E9D8A953}")
    End Function

    Public Shared Function VA_DisplayInfo() As String
        Return String.Empty
    End Function

    Public Shared Sub VA_Init1(ByRef state As Dictionary(Of String, Object), ByRef smallIntValues As Dictionary(Of String, Int16?), ByRef textValues As Dictionary(Of String, String), ByRef intValues As Dictionary(Of String, Integer?), ByRef decimalValues As Dictionary(Of String, Decimal?), ByRef booleanValues As Dictionary(Of String, Boolean?), ByRef extendedValues As Dictionary(Of String, Object))
        textValues.Add("EDLP_Version", "0.1.0")
        textValues.Add("EDLP_Initialised", "EDDB Loop Scrape " + textValues("EDLP_Version") + " Loaded")
    End Sub

    Public Shared Sub VA_Exit1(ByRef state As Dictionary(Of String, Object))

    End Sub

    Public Shared Sub VA_Invoke1(context As String, ByRef state As Dictionary(Of String, Object), ByRef smallIntValues As Dictionary(Of String, Int16?), ByRef textValues As Dictionary(Of String, String), ByRef intValues As Dictionary(Of String, Integer?), ByRef decimalValues As Dictionary(Of String, Decimal?), ByRef booleanValues As Dictionary(Of String, Boolean?), ByRef extendedValues As Dictionary(Of String, Object))

        If (context.Contains("loop config")) Then
            Dim LoopID = intValues("EDLP_LoopID")
            Dim EDDBLoopURL = "https://eddb.io/trade/loop/"
            Dim url = EDDBLoopURL + LoopID.ToString
            Dim Web = New HtmlWeb()
            Dim Doc = Web.Load(url)
            Dim systemID As Integer
            Dim stationName As String
            Dim postitionFromStar As Integer

            ' Get System 1 Name
            For Each node As HtmlNode In Doc.DocumentNode.SelectNodes("//div[contains(@class,'loop-station-left')]/a[contains(@href,'system')]")
                textValues.Add("EDLP_System1", node.ChildNodes(0).InnerHtml)
                ' Get ID for System 1
                systemID = getSystemID(node.Attributes("href").Value)
                textValues.Add("EDLP_System1_id", getSystemID(systemID))
            Next
            ' Get Station1 Name
            For Each node As HtmlNode In Doc.DocumentNode.SelectNodes("//div[contains(@class,'loop-station-left')]/a[contains(@href,'station')]")
                stationName = node.ChildNodes(0).InnerHtml
                textValues.Add("EDLP_Station1", stationName)
                ' Get Station 2 order from star
                postitionFromStar = getStationListByDistance(systemID, stationName)
                intValues.Add("EDLP_Station1Distance", postitionFromStar)
            Next
            ' Get System 2 Name
            For Each node As HtmlNode In Doc.DocumentNode.SelectNodes("//div[contains(@class,'loop-station-right')]/a[contains(@href,'system')]")
                textValues.Add("EDLP_System2", node.ChildNodes(0).InnerHtml)
                ' Get ID for System 2
                systemID = getSystemID(node.Attributes("href").Value)
                textValues.Add("EDLP_System2_id", getSystemID(systemID))
            Next
            ' Get Station 2 Name
            For Each node As HtmlNode In Doc.DocumentNode.SelectNodes("//div[contains(@class,'loop-station-right')]/a[contains(@href,'station')]")
                stationName = node.ChildNodes(0).InnerHtml
                textValues.Add("EDLP_Station2", stationName)
                ' Get Station 2 order from star
                intValues.Add("EDLP_Station2Distance", getStationListByDistance(systemID, stationName))
            Next
            ' Get name of item to buy at Station 1
            For Each node As HtmlNode In Doc.DocumentNode.SelectNodes("(//div[contains(@class,'loop-actions')]//a[contains(@href,'commodity')])[1]")
                textValues.Add("EDLP_Buy1", node.ChildNodes(0).InnerHtml)
            Next
            ' Get name of item to buy at Station 2
            For Each node As HtmlNode In Doc.DocumentNode.SelectNodes("(//div[contains(@class,'loop-actions')]//a[contains(@href,'commodity')])[2]")
                textValues.Add("EDLP_Buy2", node.ChildNodes(0).InnerHtml)
            Next
        End If

    End Sub

    Public Shared Function getSystemID(href As String) As Integer
        Dim expression As New Regex("\d+")
        Dim results = expression.Matches(href)
        For Each match As Match In results
            Return match.Value
        Next
        Return False
    End Function

    Public Shared Function getStationListByDistance(systemID As Integer, stationName As String)
        Dim EDDBSystemURL As String = "https://eddb.io/system/"
        Dim StationURL As String = EDDBSystemURL & Convert.ToString(systemID)
        Dim Web = New HtmlWeb()
        Dim Doc = Web.Load(StationURL)
        Dim dictionary = New Dictionary(Of String, Integer)()

        For Each stationNode As HtmlNode In Doc.DocumentNode.SelectNodes("//tr[count(preceding-sibling::tr[@class=""stationTypeGroup""])=1]//td")
            Dim expression As New Regex("([a-zA-Z']+ [a-zA-Z']+)\s*([ML])\s*([0-9,]+) ls")
            Dim results = expression.Matches(stationNode.InnerText)

            For Each match As Match In results
                dictionary.Add(match.Groups(1).Value, Int32.Parse(match.Groups(3).Value))
            Next
        Next

        Dim counter As Integer = 0
        For Each station As KeyValuePair(Of String, Integer) In dictionary.OrderBy(Function(key) key.Value)
            counter += 1
            If station.Key.Contains(stationName) Then
                Return counter
            End If
        Next

        Return False

    End Function

End Class
