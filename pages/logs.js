var $url = '/pages/logs';
var $urlActionsExport = '/pages/logs/actions/export';
var $urlActionsVisible = '/pages/logs/actions/visible';
var $apiUrl = utils.getQueryString('apiUrl');
var $siteId = utils.getQueryString('siteId');
var $channelId = utils.getQueryString('channelId');
var $contentId = utils.getQueryString('contentId');
var $formId = utils.getQueryString('formId');
var $returnUrl = utils.getQueryString('returnUrl');

var data = {
  pageLoad: false,
  pageAlert: null,
  pageType: null,
  fieldInfoList: null,
  allAttributeNames: [],
  listAttributeNames: [],
  isReply: false,
  page: 1,
  items: [],
  count: null,
  pages: null,
  pageOptions: null
};

var methods = {
  delete: function (logId) {
    var $this = this;

    utils.loading(true);
    $api.delete($url, {
      params: {
        siteId: $siteId,
        channelId: $channelId,
        contentId: $contentId,
        formId: $formId,
        logId: logId
      }
    }).then(function (response) {
      var res = response.data;

      $this.items = res.value;
      $this.count = res.count;
      $this.pages = res.pages;
      $this.page = res.page;
      $this.pageOptions = [];
      for (var i = 1; i <= $this.pages; i++) {
        $this.pageOptions.push(i);
      }
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  },

  btnEditClick: function (logId) {
    location.href = 'logAdd.html?siteId=' + $siteId + '&channelId=' + $channelId + '&contentId=' + $contentId + '&formId=' + $formId + '&logId=' + logId + '&apiUrl=' + encodeURIComponent($apiUrl) + '&returnUrl=' + encodeURIComponent($returnUrl);
  },

  btnReplyClick: function (logId) {
    utils.openLayer({
      title: '回复',
      url: 'logsLayerReply.html?siteId=' + $siteId + '&channelId=' + $channelId + '&contentId=' + $contentId + '&formId=' + $formId + '&logId=' + logId + '&apiUrl=' + encodeURIComponent($apiUrl)
    });
  },

  btnDeleteClick: function (logId) {
    var $this = this;

    utils.alertDelete({
      title: '删除数据',
      text: '此操作将删除数据，确定吗？',
      callback: function () {
        $this.delete(logId);
      }
    });
  },

  btnExportClick: function () {
    utils.loading(true);

    $api.post($urlActionsExport, {
      siteId: $siteId,
      channelId: $channelId,
      contentId: $contentId,
      formId: $formId
    }).then(function (response) {
      var res = response.data;

      swal2({
        toast: true,
        type: 'success',
        title: "数据导出成功",
        showConfirmButton: false,
        timer: 1000
      }).then(function () {
        window.open(res.value);
      });
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  },

  btnVisibleClick: function (attributeName) {
    var $this = this;
    event.stopPropagation();
    event.preventDefault();

    utils.loading(true);
    $api.post($urlActionsVisible, {
      siteId: $siteId,
      channelId: $channelId,
      contentId: $contentId,
      formId: $formId,
      attributeName: attributeName
    }).then(function (response) {
      var res = response.data;

      $this.listAttributeNames = res.value;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  },

  btnNavClick: function (pageName) {
    location.href = pageName + '?siteId=' + $siteId + '&channelId=' + $channelId + '&contentId=' + $contentId + '&formId=' + $formId + '&apiUrl=' + encodeURIComponent($apiUrl) + '&returnUrl=' + encodeURIComponent($returnUrl);
  },

  getAttributeText: function (attributeName) {
    if (attributeName === 'AddDate') {
      return '添加时间';
    }
    return attributeName;
  },

  getAttributeValue: function (item, attributeName) {
    return item[_.camelCase(attributeName)];
  },

  loadFirstPage: function () {
    if (this.page === 1) return;
    this.loadPage(1);
  },

  loadPrevPage: function () {
    if (this.page - 1 <= 0) return;
    this.loadPage(this.page - 1);
  },

  loadNextPage: function () {
    if (this.page + 1 > this.pages) return;
    this.loadPage(this.page + 1);
  },

  loadLastPage: function () {
    if (this.page + 1 > this.pages) return;
    this.loadPage(this.pages);
  },

  onPageSelect(option) {
    this.loadPage(option);
  },

  loadPage: function (page) {
    var $this = this;

    if ($this.pageLoad) {
      utils.loading(true);
    }

    $api.get($url, {
      params: {
        siteId: $siteId,
        channelId: $channelId,
        contentId: $contentId,
        formId: $formId,
        page: page
      }
    }).then(function (response) {
      var res = response.data;

      if ($this.pageLoad) {
        utils.loading(false);
        utils.scrollToTop();
      } else {
        $this.pageLoad = true;
      }

      $this.fieldInfoList = res.fieldInfoList;
      $this.allAttributeNames = res.allAttributeNames;
      $this.listAttributeNames = res.listAttributeNames;
      $this.isReply = res.isReply;

      $this.items = res.value;
      $this.count = res.count;
      $this.pages = res.pages;
      $this.page = res.page;
      $this.pageOptions = [];
      for (var i = 1; i <= $this.pages; i++) {
        $this.pageOptions.push(i);
      }
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  }
};

Vue.component("multiselect", window.VueMultiselect.default);

new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.loadPage(1);
  }
});
