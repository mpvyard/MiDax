﻿<!DOCTYPE html>
<html lang="en"><head><meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
    <meta charset="utf-8">
    <title>MiDAX</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
    <meta name="description" content="Trading Signals"/>
    <meta name="author" content="Bitlsoft"/>
    <meta property="og:url" content="http://bitlsoft.com/midax.aspx" />
    <meta property="og:title" content="MiDAX" />
    <meta property="og:description" content="Trading Signals" />
    <meta property="og:image" content="http://bitlsoft.com/images/logo.png" />

    <!-- official bootstrap styles -->
    <link rel="stylesheet" href="//netdna.bootstrapcdn.com/bootstrap/3.1.1/css/bootstrap.min.css">
    <link rel="stylesheet" href="//netdna.bootstrapcdn.com/bootstrap/3.1.1/css/bootstrap-theme.min.css">

    <!-- Le styles -->
    <link rel="stylesheet" href="css/bootstrap/style.css"/>
    <link href="css/bootstrap/bootstrap.css" rel="stylesheet">
    
    <style type="text/css">
      body {
        padding-top: 20px;
        padding-bottom: 60px;
      }

      /* Custom container */
      .container {
        margin: 0 auto;
        max-width: 1000px;
      }
      .container > hr {
        margin: 60px 0;
      }

      /* Main marketing message and sign up button */
      .jumbotron {
        margin: 80px 0;
        text-align: center;
      }
      .jumbotron h1 {
        font-size: 100px;
        line-height: 1;
      }
      .jumbotron .lead {
        font-size: 24px;
        line-height: 1.25;
      }
      .jumbotron .btn {
        font-size: 21px;
        padding: 14px 24px;
      }

      /* Supporting marketing content */
      .marketing {
        margin: 60px 0;
      }
      .marketing p + h4 {
        margin-top: 28px;
      }


      /* Customize the navbar links to be fill the entire space of the .navbar */
      .navbar .navbar-inner {
        padding: 0;
      }
      .navbar .nav {
        margin: 0;
        display: table;
        width: 100%;
      }
      .navbar .nav li {
        display: table-cell;
        width: 1%;
        float: none;
      }
      .navbar .nav li a {
        font-weight: bold;
        text-align: center;
        border-left: 1px solid rgba(255,255,255,.75);
        border-right: 1px solid rgba(0,0,0,.1);
      }
      .navbar .nav li:first-child a {
        border-left: 0;
        border-radius: 3px 0 0 3px;
      }
      .navbar .nav li:last-child a {
        border-right: 0;
        border-radius: 0 3px 3px 0;
      }     

      body {
          font: 10px sans-serif;
        }

        .axis path,
        .axis line {
          fill: none;
          stroke: #000;
          shape-rendering: crispEdges;
        }

        .x.axis path {
          display: none;
        }

        .line {
          fill: none;
          stroke: steelblue;
          stroke-width: 1.5px;
        }

        .legend rect {
          fill:white;
          stroke:black;
          opacity:0.8;
        }        
    </style>
    <link href="css/bootstrap/bootstrap-responsive.css" rel="stylesheet">

    <!-- HTML5 shim, for IE6-8 support of HTML5 elements -->
    <!--[if lt IE 9]>
      <script src="../assets/js/html5shiv.js"></script>
    <![endif]-->

    <!-- Fav and touch icons -->
    <link rel="apple-touch-icon-precomposed" sizes="144x144" href="ico/apple-touch-icon-144-precomposed.png">
    <link rel="apple-touch-icon-precomposed" sizes="114x114" href="ico/apple-touch-icon-114-precomposed.png">
    <link rel="apple-touch-icon-precomposed" sizes="72x72" href="ico/apple-touch-icon-72-precomposed.png">
    <link rel="apple-touch-icon-precomposed" href="ico/apple-touch-icon-57-precomposed.png">
    <link rel="shortcut icon" href="ico/favicon.png">
    <link rel="icon" type="image/png" href="images/favicon.ico" />

    <script src="jscript/jquery-1.9.1.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/d3/3.5.5/d3.min.js"></script>
	<script type="text/javascript" src="jscript/d3.legend.js"></script>
    <script type="text/javascript" src="jscript/Init.js"></script>
    <script type="text/javascript" src="jscript/Modal.js"></script>
    <script type="text/javascript" src="jscript/Midax.js"></script> 
    <script type="text/javascript" src="jscript/highstock-all.js"></script> 
    <script type="text/javascript" src="jscript/MarkitTimeseriesServiceSample.js"></script>        
    <!--[if lte IE 9]>
    <script type='text/javascript' src='//cdnjs.cloudflare.com/ajax/libs/jquery-ajaxtransport-xdomainrequest/1.0.0/jquery.xdomainrequest.min.js'></script>
    <![endif]--> 
    <link rel="stylesheet" href="//code.jquery.com/ui/1.11.4/themes/smoothness/jquery-ui.css">
    <script src="//code.jquery.com/ui/1.11.4/jquery-ui.js"></script>
    <script>
      function formatDate(date) {
          return date.getFullYear() + "-" + (date.getMonth() + 1)  + "-" + date.getDate();
      }

      $(function () {
          $("#datepicker").datepicker();
          $("#datepickerend").datepicker();
      });
    </script>
    <script type="text/javascript">
        $(document).ready(function () { 
            $("#GO").click(function () {
                var currentDate = $("#datepicker").datepicker("getDate");
                if (currentDate == null)
                    currentDate = new Date();
                var endDate = $("#datepickerend").datepicker("getDate");
                if (endDate == null)
                    endDate = new Date(currentDate);

                var profits = null;
                var sync = {};
                sync['nbDays'] = 0;
                sync['processedDays'] = 0;
                sync['profits'] = null;
                var cd = new Date(currentDate);
                while (cd <= endDate) {
                    sync['nbDays'] = sync['nbDays'] + 1;
                    do {
                        var newDate = cd.setDate(cd.getDate() + 1);
                        cd = new Date(newDate);
                    } while (cd.getDay() == 6 || cd.getDay() == 0);
                }
                while (currentDate <= endDate) {
                    var genericParams = { "begin": formatDate(currentDate) + " " + $('#timestart option:selected').text(), "end": formatDate(currentDate) + " " + $('#timestop option:selected').text() };

                    var equity = $("#equity").val();
                    var equityParams = $.extend({ "stockid": equity }, genericParams);

                    var requests = { "GetStockData0": equityParams };
                    var isVolume = false;
                    if (window.document.getElementById("indicator").selectedIndex > 0) {
                        var indicatorIds = $("#indicator").val().split('#');
                        var idx = 0;
                        for (var id in indicatorIds) {
                            if (indicatorIds[id].indexOf("Volume") != -1)
                                isVolume = true;
                            var indicatorParams = $.extend({ "indicatorid": indicatorIds[id] + "_" + $("#equity").val() }, genericParams);
                            if (indicatorIds[id].startsWith("Low") || indicatorIds[id].startsWith("High") || indicatorIds[id].startsWith("Close")) {
                                var prevDate = new Date((new Date(currentDate)).setDate(currentDate.getDate() - 1));
                                indicatorParams["begin"] = formatDate(prevDate) + " " + $('#timestart option:selected').text();
                                indicatorParams["end"] = formatDate(prevDate) + " " + $('#timestop option:selected').text();
                            }
                            var key = "GetIndicatorData";
                            key = key.concat(idx.toString());
                            var newDict = {};
                            newDict[key] = indicatorParams;
                            $.extend(requests, newDict);
                            idx++;
                        }
                    }
                    if (isVolume)
                        delete requests["GetStockData0"];
                    if (window.document.getElementById("signal").selectedIndex > 0) {
                        var signalParams = $.extend({ "signalid": $("#signal").val() + "_" + $("#equity").val() }, genericParams);
                        $.extend(requests, { "GetSignalData0": signalParams });
                    }

                    MidaxAPI(requests, sync);
                    
                    /*
                    if (equity.startsWith("MARKIT:")) {
                        equity = equity.substring(7);
                        var markitChart = new Markit.InteractiveChartApi(equity, 14);
                    }*/

                    do {
                        var newDate = currentDate.setDate(currentDate.getDate() + 1);
                        currentDate = new Date(newDate);
                    } while (currentDate.getDay() == 6 || currentDate.getDay() == 0);
                }                    
            });
        });
    </script>
  </head>

  <body>
    <div class="container">

      <div class="masthead">        
        <h3 class="muted">
        <div><table>
            <tr><th><img alt="BitL" src="images/logo_mini.png" class="logo"/></th>
                <th style="text-align: left; padding-left: 20px"><img alt="MiDAX" src="images/midax.png" class="logo" style="width: 150px"/><br/>Trading Signals</th></tr></table></div></h3>
      </div>

      <!-- Jumbotron -->
      <div class="jumbotron" style="margin-top: 5px">   
        <div class="control-group">             
           <div class="controls">
             <table><tr><th><a>From date:</a>
             <input class="form-control input-medium" placeholder="Today" id="datepicker"></th></tr>
             <tr><th><a>To date:</a>
             <input class="form-control input-medium" placeholder="" id="datepickerend"></th></tr></table>
             <select class="combobox input-medium" id="timestart" >
               <option value="">08:00</option>
               <option value="1">08:30</option>
               <option value="2">09:00</option>
               <option value="3">10:00</option>
               <option value="4">11:00</option>
               <option value="5">12:00</option>
               <option value="6">13:00</option>
               <option value="7">14:00</option>
               <option value="8">15:00</option>
               <option value="9">16:00</option>
               <option value="10">17:00</option>
               <option value="11">18:00</option>
               <option value="12">19:00</option>
               <option value="13">20:00</option>
             </select>
             <select class="combobox input-medium" id="timestop">
               <option value="">21:30</option>
               <option value="1">21:00</option>
               <option value="2">20:00</option>
               <option value="3">19:00</option>
               <option value="4">18:00</option>
               <option value="5">17:00</option>
               <option value="6">16:00</option>
               <option value="7">15:00</option>
               <option value="8">14:00</option>
               <option value="9">13:00</option>
               <option value="10">12:00</option>
               <option value="11">11:00</option>
               <option value="12">10:00</option>
               <option value="13">09:00</option>
             </select>
               <br />
             <select class="combobox input-large" id="equity">
               <option value="IX.D.DAX.DAILY.IP">DAX</option>
               <option value="IX.D.DOW.DAILY.IP">DOW IG</option>
               <option value="IceConnection.DOW">DOW Ice</option>
               <option value="IceConnection.DOW.Volume">DOW Volumes</option>
               <option value="IX.D.CAC.DAILY.IP">CAC</option>
               <option value="IN.D.VIX.MONTH2.IP">VIX</option>
             </select>
             <select class="combobox input-large" id="indicator">
               <option value="">Choose an indicator</option>
               <option value="High#Low#CloseBid">High/Low/Close D-1</option>
               <option value="LVLPivot#LVLS1#LVLR1">Levels Pivot/S1/R1</option>
               <option value="NearestLevel">Nearest Level</option>
               <option value="WMA_2#WMA_10#WMA_60">WMA 2mn/10mn/1hr</option>
               <option value="WMA_7#WMA_20#WMA_60">WMA 7mn/20mn/1hr</option>
               <option value="WMA_10#WMA_30#WMA_90">WMA 10mn/30mn/1hr30</option>
               <option value="VWMA_10#VWMA_30#VWMA_90">VWMA 10mn/30mn/1hr30</option>
               <option value="EMA_1#EMA_5#EMA_10">EMA 1mn/5mn/10mn</option>
               <option value="EMA_10#EMA_30#EMA_90">EMA 10mn/30mn/1hr30</option>
               <option value="WMVol_10">WM Vol 10mn</option>
               <option value="WMVol_90">WM Vol 1h30</option>    
               <option value="VWMVol_10">VWM Vol 10mn</option>
               <option value="VWMVol_90">VWM Vol 1h30</option>    
               <option value="Volume_30">Volumes 30mn</option>  
               <option value="Volume_90">Volumes 1h30</option>                           
             </select>   
             <select class="combobox input-large" id="signal">
               <option value="">Choose a signal</option>
               <option value="MacDV_10_60">MacD 10mn/1h</option>
               <option value="MacDV_30_90">MacD 30mn/1h30</option>
               <option value="MacDCas_10_30_90_100">MacD Cascade 10mn/30mn/1h30 1pt</option>
               <option value="MacDCas_10_30_90_200">MacD Cascade 10mn/30mn/1h30 2pts</option>
               <option value="Mole_2_10_60">Mole 2mn/10mn/1h</option>
               <option value="Mole_2_10_90">Mole 2mn/10mn/1h30</option>
               <option value="Mole_7_20_60">Mole 7mn/20mn/1h</option>
               <option value="ANN_WMA_4_2_1">ANN WMA</option>
             </select>          

             <button type="button" id="GO" class="btn btn-primary">
                    <span class="glyphicon glyphicon-ok"></span> </button>
            <div>
                <button type="button" id="Refresh" class="btn btn-danger btn-sm" onclick="document.location.reload();" style="float:right">
                    <span class="glyphicon glyphicon-refresh"></span>Clear!</button>
            </div>
           </div>
        </div>  
      </div>

      <div id="graphs">
      </div>

      <hr/>

      <div class="footer">
        <p>© B.I.T.L. 2015</p>
      </div>

    </div> <!-- /container -->

    <!-- Le javascript
    ================================================== -->
    <!-- Placed at the end of the document so the pages load faster -->
    <script src="./Template · Bootstrap_files/jquery.js"></script>
    <script src="./Template · Bootstrap_files/bootstrap-transition.js"></script>
    <script src="./Template · Bootstrap_files/bootstrap-alert.js"></script>
    <script src="./Template · Bootstrap_files/bootstrap-modal.js"></script>
    <script src="./Template · Bootstrap_files/bootstrap-dropdown.js"></script>
    <script src="./Template · Bootstrap_files/bootstrap-scrollspy.js"></script>
    <script src="./Template · Bootstrap_files/bootstrap-tab.js"></script>
    <script src="./Template · Bootstrap_files/bootstrap-tooltip.js"></script>
    <script src="./Template · Bootstrap_files/bootstrap-popover.js"></script>
    <script src="./Template · Bootstrap_files/bootstrap-button.js"></script>
    <script src="./Template · Bootstrap_files/bootstrap-collapse.js"></script>
    <script src="./Template · Bootstrap_files/bootstrap-carousel.js"></script>
    <script src="./Template · Bootstrap_files/bootstrap-typeahead.js"></script>
      
</body></html>