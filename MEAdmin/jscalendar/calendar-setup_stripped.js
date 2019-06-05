/*  Copyright Mihai Bazon, 2002-2004  |  http://dynarch.com/mishoo/
 * -----------------------------------------------------------------
 *
 * The DHTML Calendar
 *
 * Details and latest version at:
 * http://dynarch.com/mishoo/calendar.epl
 *
 * Licensed to Discovery Productions, Inc.
 */
 Calendar.setup=function(params){function param_default(pname,def){if(typeof params[pname]=="undefined"){params[pname]=def;}};param_default("inputField",null);param_default("displayArea",null);param_default("button",null);param_default("eventName","click");param_default("ifFormat","%Y/%m/%d");param_default("daFormat","%Y/%m/%d");param_default("singleClick",true);param_default("disableFunc",null);param_default("dateStatusFunc",params["disableFunc"]);param_default("mondayFirst",true);param_default("align","Bl");param_default("range",[1900,2999]);param_default("weekNumbers",true);param_default("flat",null);param_default("flatCallback",null);param_default("onSelect",null);param_default("onClose",null);param_default("onUpdate",null);param_default("date",null);param_default("showsTime",false);param_default("timeFormat","24");param_default("electric",true);var tmp=["inputField","displayArea","button"];for(var i in tmp){if(typeof params[tmp[i]]=="string"){params[tmp[i]]=document.getElementById(params[tmp[i]]);}}if(!(params.flat||params.inputField||params.displayArea||params.button)){alert("Calendar.setup:\n  Nothing to setup (no fields found).  Please check your code");return false;}function onSelect(cal){var update=(cal.dateClicked||params.electric);if(update&&cal.params.flat){if(typeof cal.params.flatCallback=="function"){cal.params.flatCallback(cal);}else{alert("No flatCallback given -- doing nothing.");}return false;}if(update&&cal.params.inputField){cal.params.inputField.value=cal.date.print(cal.params.ifFormat);}if(update&&cal.params.displayArea){cal.params.displayArea.innerHTML=cal.date.print(cal.params.daFormat);}if(update&&cal.params.singleClick&&cal.dateClicked){cal.callCloseHandler();}if(update&&typeof cal.params.onUpdate=="function"){cal.params.onUpdate(cal);}};if(params.flat!=null){if(typeof params.flat=="string")params.flat=document.getElementById(params.flat);if(!params.flat){alert("Calendar.setup:\n  Flat specified but can't find parent.");return false;}var cal=new Calendar(params.mondayFirst,params.date,params.onSelect||onSelect);cal.showsTime=params.showsTime;cal.time24=(params.timeFormat=="24");cal.params=params;cal.weekNumbers=params.weekNumbers;cal.setRange(params.range[0],params.range[1]);cal.setDateStatusHandler(params.dateStatusFunc);cal.create(params.flat);cal.show();return false;}var triggerEl=params.button||params.displayArea||params.inputField;triggerEl["on"+params.eventName]=function(){var dateEl=params.inputField||params.displayArea;var dateFmt=params.inputField?params.ifFormat:params.daFormat;var mustCreate=false;var cal=window.calendar;if(!window.calendar){window.calendar=cal=new Calendar(params.mondayFirst,params.date,params.onSelect||onSelect,params.onClose||function(cal){cal.hide();});cal.showsTime=params.showsTime;cal.time24=(params.timeFormat=="24");cal.weekNumbers=params.weekNumbers;mustCreate=true;}else{cal.hide();}cal.setRange(params.range[0],params.range[1]);cal.params=params;cal.setDateStatusHandler(params.dateStatusFunc);cal.setDateFormat(dateFmt);if(mustCreate)cal.create();cal.parseDate(dateEl.value||dateEl.innerHTML);cal.refresh();cal.showAtElement(params.button||params.displayArea||params.inputField,params.align);return false;};};