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

    Public Shared Sub VA_StopCommand()

    End Sub

    Public Shared Sub VA_Init1(vaProxy As Object)
        vaProxy.SetText("EDLP_Version", "0.2.0")
        vaProxy.SetText("EDLP_Initialised", "EDDB Loop Scrape " + vaProxy.GetText("EDLP_Version") + " Loaded")
    End Sub

    Public Shared Sub VA_Exit1(vaProxy As Object)

    End Sub

    Public Shared Sub VA_Invoke1(vaProxy As Object)

        If (vaProxy.Context = "loop config") Then
            Dim LoopID = vaProxy.GetInt("EDLP_LoopID")
            Dim EDDBLoopURL = "https://eddb.io/trade/loop/"
            Dim url = EDDBLoopURL + LoopID.ToString
            Dim Web = New HtmlWeb()
            Dim Doc = Web.Load(url)
            Dim systemID As Integer
            Dim stationName As String
            Dim stationID As Integer
            Dim postitionFromStar As Integer
            Dim postitionFromStar2 As Integer

            ' Get System 1 Name
            For Each node As HtmlNode In Doc.DocumentNode.SelectNodes("//div[contains(@class,'loop-station-left')]/a[contains(@href,'system')]")
                vaProxy.SetText("EDLP_System1", node.ChildNodes(0).InnerHtml)
                ' Get ID for System 1
                systemID = getID(node.Attributes("href").Value)
                vaProxy.SetInt("EDLP_System1_id", systemID)
            Next
            ' Get Station1 Name
            For Each node As HtmlNode In Doc.DocumentNode.SelectNodes("//div[contains(@class,'loop-station-left')]/a[contains(@href,'station')]")
                stationName = node.ChildNodes(0).InnerHtml
                stationID = getID(node.Attributes("href").Value)
                vaProxy.SetText("EDLP_Station1", stationName)
                vaProxy.SetInt("EDLP_Station1_id", stationID)
                ' Get Station 2 order from star
                If (getStationListByDistance(vaProxy, systemID, stationName)) Then
                    postitionFromStar = getStationListByDistance(vaProxy, systemID, stationName)
                    vaProxy.SetInt("EDLP_Station1Distance", postitionFromStar)
                End If
            Next
            ' Get System 2 Name
            For Each node As HtmlNode In Doc.DocumentNode.SelectNodes("//div[contains(@class,'loop-station-right')]/a[contains(@href,'system')]")
                vaProxy.SetText("EDLP_System2", node.ChildNodes(0).InnerHtml)
                ' Get ID for System 2
                systemID = getID(node.Attributes("href").Value)
                vaProxy.SetInt("EDLP_System2_id", systemID)
            Next
            ' Get Station 2 Name
            For Each node As HtmlNode In Doc.DocumentNode.SelectNodes("//div[contains(@class,'loop-station-right')]/a[contains(@href,'station')]")
                stationName = node.ChildNodes(0).InnerHtml
                stationID = getID(node.Attributes("href").Value)
                vaProxy.SetText("EDLP_Station2", stationName)
                vaProxy.SetInt("EDLP_Station2_id", stationID)
                ' Get Station 2 order from star
                If (getStationListByDistance(vaProxy, systemID, stationName)) Then
                    postitionFromStar2 = getStationListByDistance(vaProxy, systemID, stationName)
                    vaProxy.SetInt("EDLP_Station2Distance", postitionFromStar2)
                End If
            Next
            ' Get name of item to buy at Station 1
            For Each node As HtmlNode In Doc.DocumentNode.SelectNodes("(//div[contains(@class,'loop-actions')]//a[contains(@href,'commodity')])[1]")
                vaProxy.SetText("EDLP_Buy1", node.ChildNodes(0).InnerHtml)
            Next
            ' Get name of item to buy at Station 2
            For Each node As HtmlNode In Doc.DocumentNode.SelectNodes("(//div[contains(@class,'loop-actions')]//a[contains(@href,'commodity')])[2]")
                vaProxy.SetText("EDLP_Buy2", node.ChildNodes(0).InnerHtml)
            Next
        End If

    End Sub

    Public Shared Function getID(href As String) As Integer
        Dim expression As New Regex("\d+")
        Dim results = expression.Matches(href)
        For Each match As Match In results
            Return match.Value
        Next
        Return False
    End Function

    Public Shared Function getStationListByDistance(vaProxy As Object, systemID As Integer, stationName As String)
        Dim EDDBSystemURL As String = "https://eddb.io/system/"
        Dim StationURL As String = EDDBSystemURL & Convert.ToString(systemID)
        Dim Web = New HtmlWeb()
        Dim Doc = Web.Load(StationURL)
        Dim dictionary = New Dictionary(Of String, Integer)()
        Dim distance As Int32
        Dim cleanValue As String
        For Each stationNode As HtmlNode In Doc.DocumentNode.SelectNodes("//tr[count(preceding-sibling::tr[@class=""stationTypeGroup""])=1]//td")
            Dim expression As New Regex("([a-zA-Z']+ [a-zA-Z']+)\s*([ML])\s*([0-9,]+) ls")
            Dim results = expression.Matches(stationNode.InnerText)

            For Each match As Match In results
                If match.Groups(1).Value IsNot Nothing And match.Groups(3).Value IsNot Nothing Then
                    cleanValue = match.Groups(3).Value.Replace(",", "")
                    If Int32.TryParse(cleanValue, distance) Then
                        dictionary.Add(match.Groups(1).Value, Int32.Parse(cleanValue))
                    Else
                        vaProxy.WriteToLog("EDDB Scraper Error: Cannot detect distance for " + stationName + " (" + cleanValue + ")", "red")
                        Return False
                    End If

                End If

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