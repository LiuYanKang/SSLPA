var treemenu, mainTabs;

$.page_load = function () {
    if (!$.userinfo()) return;
    $("#lblUsername").text($.userinfo().Name || "用户");

    mainTabs = mini.get('mainTabs');
    treemenu = mini.get('treemenu');

    $.srv("Base", "GetUserMenu", {
        success: function (data) {
            var menuData = data.Data;

            // 加工菜单图标
            $(menuData).each(function () {
                this.icon = this.Icon || "icon-file";
                this.color = this.Color || "grey";
                this.url = this.Url;

                var cls = this.icon + " " + this.color;
                this.text = "<i class='" + cls + " icon-large'></i> " + this.Name;
            });
            treemenu.loadList(menuData);
            // 默认选中第一个
            //if (menuData.length > 0) {
            //    var firstNode = menuData[0];
            //    treemenu.selectNode(firstNode);
            //    onNodeSelect({ node: firstNode });
            //}
        }
    });
};

function onNodeSelect(e) {
    if (!e.node.Url) return;
    var tabName = "tab" + e.node.MenuCode;
    var tab = mainTabs.getTab(tabName);
    if (!tab) {
        e.node.icon = e.node.icon || "icon-file";
        e.node.color = e.node.color || "grey";
        var title = "<i class='" + e.node.icon + " " + e.node.color + " icon-large'></i> " + e.node.Name;
        tab = {
            name: tabName,
            title: title,
            showCloseButton: true,
            url: e.node.Url
        };
        mainTabs.addTab(tab);
    }

    mainTabs.activeTab(tab);
}

function onTabsActiveChanged(e) {
    mini.layout();
}

function changePwd() {
    mini.open({
        title: "修改密码",
        url: bootPATH + "Base/ChangePwd.html",
        width: 370,
        height: 220,
        ondestroy: function (action) {
            if (action != "save") return;

            mini.showTips({ content: "密码修改成功。<p>3秒钟后跳转到登陆页面……</p>", state: "success", timeout: 5000 });
            setTimeout(function () { $.logout(); }, 3000);
        }
    });
}

var currentTab = null, tabs = null;
function onBeforeOpen(e) {
    tabs = mini.get("mainTabs");
    currentTab = tabs.getTabByEvent(e.htmlEvent);
    if (!currentTab) {
        e.cancel = true;
    } else {
        tabs.activeTab(currentTab);
    };
};

function onItemClick(e) {
    var item = e.sender, name = item.name, status = 0;
    var seclctedTabIndex = tabs.activeIndex;
    switch (name) {
        case "refresh":
            tabs.reloadTab(currentTab);
            break;
        case "close":
            if (currentTab.name != "homePage") {
                tabs.removeTab(currentTab);
            };
            break;
        case "closeAll":
            var but = tabs.getTab("homePage");
            tabs.removeAll(but);
            break;
        case "closeAllWithoutCurrent":
            var but = [currentTab];
            but.push(tabs.getTab("homePage"));
            tabs.removeAll(but);
            break;
        case "closeLeftWithoutCurrent":
            status = 1;
            break;
        case "closeRightWithoutCurrent":
            status = 2;
            break;
        default:
            break;
    };
    var tabCount = tabs.tabs.length;
    for (var i = tabCount - 1; i >= 1; i--) {//保留首页
        if ((status == 1 && i < seclctedTabIndex) || (status == 2 && i > seclctedTabIndex)) {
            tabs.removeTab(tabs.tabs[i]);
        };
    };
};



function onDefaultRoomChange() {
    mini.open({
        title: "请选择当前操作厅...",
        url: bootPATH + "BaseInfo/WaitingRoomSel.html?multiSelect=0",
        width: 500,
        height: 425,
        onload: function () {
            var iframe = this.getIFrameEl();
            var defaultRoom = SS.defaultRoom();
            if (defaultRoom) {
                var selected = { RoomID: defaultRoom.id, RoomName: defaultRoom.name };
                iframe.contentWindow.setData(selected);
            }
        },
        ondestroy: function (action) {
            if (action == "ok") {
                var iframe = this.getIFrameEl();
                var data = iframe.contentWindow.getData();
                if (data.length < 1) return false;
                data = mini.clone(data[0]);
                SS.defaultRoom(data.RoomID, data.RoomName);
                $('#lblDefaultRoom').text(data.RoomName);
            }
        }
    });
}

function onLogout() {
    mini.confirm("确定注销？", "确定？", function (action) {
        if (action == "ok") {
            $.logout()
        }
    });
}