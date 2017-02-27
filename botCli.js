//通过Directline 3.0 与 bot连接
//--------------------常量声明----------------
var constant = {
//	url:"http://114.251.168.251:8080"
	url : "http://worko.longfor.com:59650",
	key : "你的directline 密钥",
	directlineUrl : "https://directline.botframework.com"
}
//--------------------变量声明----------------
//会话信息－创建会话后产生
var conversations = {
  "conversationId": "string",
  "token": "string",
  "expires_in": 0,
  "streamUrl": "string",
  "eTag": "string"
}
//声明webSocket对象,用于与Bot建立链接
var ws = null;
//活动信息
var activity = {
	"type": "message",
	"channelId": "directline",
	"from": {
		"id": "token",//用户的id
		"name": "lucl" //当前会话的用户名
	},
	"text": ""
}

/**
 * bot客户端操作API－－－－－－－－－－－－－－－－－－－－－－－－－－－－－－－－－－－－－－
 */
var botCli= {
	//--------------------初始化bot客户端-----------------------
	init : function(){
		//1、单点登陆
//		common.login();
//		activity.from.name=loginUser.data.usercode;
		var para=common.getUrlParams();
		//2、创建会话
		this.createConversations();
		//3、建立webSocket连接
		ws = new WebSocket(conversations.streamUrl);
	},
	//--------------------重置客户端输入框高度-----------------------
	resetHeight:function(){
		var obj=$("#sendMsg")[0];
		if(obj.scrollHeight>34 && obj.scrollHeight<100){
			$(obj).parent().height(obj.scrollHeight);
			$(obj).parent().parent().parent().height(14+obj.scrollHeight);
		}
		if($(obj).val()==""){
			$(obj).parent().height(34);
			$(obj).parent().parent().parent().height(48);
		}
		console.log(obj.scrollHeight)
	},
	//--------------------创建bot会话-----------------------
	createConversations : function(){
		//开始新的会话 
		$.ajax({
				url: constant.directlineUrl + "/v3/directline/conversations",
				beforeSend: function(xhrObj) {
					// Request headers
					xhrObj.setRequestHeader("Accept",
						"application/json");
					xhrObj.setRequestHeader("Content-Type",
						"application/json; charset = utf-8");
					xhrObj
						.setRequestHeader("Authorization",
							"Bearer "+constant.key);
				},
				type: "POST",
				async : false,
			}).done(function(data) {
				//会话信息赋值
				conversations=data;
				console.log(conversations);
				//把会话id存入localStorage
				localStorage.conversationsId=conversations.conversationId;
			}).fail(function(e) {
				console.log(e);
			});
	},
	//--------------------发送文件给bot-----------------------
	upload : function(){
		$.ajax(
				{
					url : constant.directlineUrl+"/v3/directline/conversations/"+params
					      +"/stream?t="+streamurl,
					beforeSend : function(xhrObj) {
						// Request headers
						xhrObj.setRequestHeader("Accept",
								"application/json");
						xhrObj.setRequestHeader("Content-Type",
								"application/json; charset = utf-8");
						xhrObj
								.setRequestHeader("Authorization",
										"Bearer 8MNhtddtwrk.cwA.iHY.lblyc-cTEXIrWNx7pEKFxEtgk6jNY22SJ6pln5srN-I");
					},
					type : "GET",
					data :{
					    "type": "message",
					    "from": {
					        "id": "user1"
					    },
					    "text": "明天有没有时间"
					},
				}).done(function(data) {
			console.log(data);
		}).fail(function(e) {
			console.log(e);
		}); 
	},
	//--------------------获取历史会话活动-----------------------
	getActivities : function(){
		$.ajax({
				url: constant.directlineUrl+"/v3/directline/conversations/"+localStorage.conversationsId+"/activities",
				beforeSend: function(xhrObj) {
					// Request headers
					xhrObj.setRequestHeader("Accept",
						"application/json");
					xhrObj.setRequestHeader("Content-Type",
						"application/json; charset = utf-8");
					xhrObj
					.setRequestHeader("Authorization",
						"Bearer "+constant.key);
				},
				type: "GET"
			}).done(function(data) {
				console.log(data);
				conversations=data;
			}).fail(function(e) {
				console.log(e);
			});
	},
	//--------------------发送文本给bot-----------------------
	postActivities : function(){
		activity.text=$("#sendMsg").val();
		htmlCreater.cbotHtmlMine(activity.text);
		$("#sendMsg").val("");
		//重置输入框高度
		this.resetHeight();
		console.log(activity);
		console.log(localStorage.conversationsId+"---------");
		$.ajax({
				url: constant.directlineUrl+"/v3/directline/conversations/"+localStorage.conversationsId+"/activities",
				beforeSend: function(xhrObj) {
					// Request headers
					xhrObj.setRequestHeader("Accept",
						"application/json");
					xhrObj.setRequestHeader("Content-Type",
						"application/json;charset=UTF-8");
					xhrObj
					.setRequestHeader("Authorization",
						"Bearer "+constant.key);
				},
				type: "POST",
				data : JSON.stringify(activity)
			}).done(function(data) {
				console.log(data);
			}).fail(function(e) {
				console.log(e);
			});
	}
}

/**
 * webSocket与bot连接的监听回调方法－－－－－－－－－－－－－－－－－－－－－－－－－－－－－－－－－－－－－－
 */
ws.onopen = function(e){//监听连接是否打开
	console.log("Connection open...");
	htmlCreater.cbotTip(new Date().Format("MM-dd hh:ss")+' 创建链接成功');
	htmlCreater.cbotHtmlOther("您好，我是龙湖小智，有什么事请吩咐。");
}

ws.onmessage = function(e) {//监听是否有消息接收
    if(typeof e.data === "string"){
        console.log("String message received");
        if(e.data!=""){
        		var jsond=JSON.parse(e.data);
        		console.log(jsond);
//      		if(jsond.activities[0].from.name!=loginUser.data.usercode){
//      			htmlCreater.cbotHtmlOther(jsond.activities[0].text);
//      		}
        }else{
        		console.log(e.data)
        }
    } else {
        console.log("Other message received", e, e.data);
    }
};
ws.onerror = function(e){//监听是否异常产生
    console.log('websocked error');
}
ws.onclose = function(e) {//监听链接是否关闭
    console.log("Connection closed", e);
};







/**
 * 组装会话内容html，并显示在页面上－－－－－－－－－－－－－－－－－－－－－－－－－－－－－－－－－－－－－－
 */
var htmlCreater = {
	//--------------------bot方会话html-----------------------
	cbotHtmlOther : function(data){
		var html ='';
		html += '<li class="msg-content msg-user-other page-wrap">';
				html += '<div class="left">';
					html += '<img src="img/bot.png" />';
				html += '</div>';
				html += '<div class="right">';
					html += '<div class="content">';
						html += data;
							html += '<span class="icon-backwardfill"></span>';
				html += '</div></div></li>';
				
				$("#msg-list").append(html);
	},
	//--------------------发送者方会话html-----------------------
	cbotHtmlMine : function(data){
		var html ='';
		html += '<li class="msg-content msg-user-mine page-wrap wrap-end">';
				html += '<div class="left">';
					html += '<div class="content">';
							html += data;
							html += '<span class="icon-forwardfill"></span>';
						html += '</div>';
					html += '</div>';
					html += '<div class="right">';
						html += '<img src="http://worko.longfor.com:59650/workofile/head/'+loginUser.data.headIcon+'.jpg" onerror="this.src=\'img/nohead.png\'" />';
					html += '</div>';
				html += '</li>';
		$("#msg-list").append(html);
	},
	//--------------------时间信息-----------------------
	cbotTip : function(data){
		$("#msg-list").append('<li class="msg-time"><span>'+data+'</span></li>')
	}
}
