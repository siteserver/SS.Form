config.apiUrl = utils.getQueryString('apiUrl');
config.siteId = utils.getQueryString('siteId');
config.channelId = utils.getQueryString('channelId');
config.contentId = utils.getQueryString('contentId');
config.formId = utils.getQueryString('formId');
config.returnUrl = utils.getQueryString('returnUrl');

var $api = new utils.Api('/ss.form/logs');

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
  pageOptions: null,
};

var methods = {
  delete: function (logId) {
    var $this = this;

    utils.loading(true);
    $api.delete({
      siteId: config.siteId,
      channelId: config.channelId,
      contentId: config.contentId,
      formId: config.formId,
      logId: logId
    }, function (err, res) {
      utils.loading(false);
      if (err || !res || !res.value) return;

      $this.items = res.value;
      $this.count = res.count;
      $this.pages = res.pages;
      $this.page = res.page;
      $this.pageOptions = [];
      for (var i = 1; i <= $this.pages; i++) {
        $this.pageOptions.push(i);
      }
    });
  },

  btnEditClick: function (logId) {
    location.href = 'logAdd.html?siteId=' + config.siteId + '&channelId=' + config.channelId + '&contentId=' + config.contentId + '&formId=' + config.formId + '&logId=' + logId + '&apiUrl=' + encodeURIComponent(config.apiUrl) + '&returnUrl=' + encodeURIComponent(config.returnUrl);
  },

  btnReplyClick: function (logId) {
    utils.openLayer({
      title: '回复',
      url: 'logsLayerReply.html?siteId=' + config.siteId + '&channelId=' + config.channelId + '&contentId=' + config.contentId + '&formId=' + config.formId + '&logId=' + logId + '&apiUrl=' + encodeURIComponent(config.apiUrl)
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
    $api.post({
      siteId: config.siteId,
      channelId: config.channelId,
      contentId: config.contentId,
      formId: config.formId
    }, function (err, res) {
      utils.loading(false);
      if (err || !res || !res.value) return;

      alert({
        toast: true,
        type: 'success',
        title: "数据导出成功",
        showConfirmButton: false,
        timer: 1000
      }).then(function () {
        window.open(res.value);
      });
    }, 'actions/export');
  },

  btnVisibleClick: function (attributeName) {
    var $this = this;
    event.stopPropagation();
    event.preventDefault();

    utils.loading(true);
    $api.post({
      siteId: config.siteId,
      channelId: config.channelId,
      contentId: config.contentId,
      formId: config.formId,
      attributeName: attributeName
    }, function (err, res) {
      utils.loading(false);
      if (err || !res || !res.value) return;

      $this.listAttributeNames = res.value;
    }, 'actions/visible');
  },

  btnNavClick: function (pageName) {
    location.href = pageName + '?siteId=' + config.siteId + '&channelId=' + config.channelId + '&contentId=' + config.contentId + '&formId=' + config.formId + '&apiUrl=' + encodeURIComponent(config.apiUrl) + '&returnUrl=' + encodeURIComponent(config.returnUrl);
  },

  getAttributeText: function (attributeName) {
    if (attributeName === 'AddDate') {
      return '添加时间';
    } else if (attributeName === 'IsReplied') {
      return '是否回复';
    } else if (attributeName === 'ReplyDate') {
      return '回复时间';
    } else if (attributeName === 'ReplyContent') {
      return '回复内容';
    }

    return attributeName;
  },

  getAttributeValue: function (item, attributeName) {
    if (attributeName === 'IsReplied') {
      return item.isReplied ? '<strong class="text-primary">已回复</strong>' : '<strong class="text-danger">未回复</strong>';
    } else if (attributeName === 'ReplyDate') {
      return item.isReplied ? item.replyDate : '';
    } else if (attributeName === 'ReplyContent') {
      return item.isReplied ? item.replyContent : '';
    }

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

    $api.get({
      siteId: config.siteId,
      channelId: config.channelId,
      contentId: config.contentId,
      formId: config.formId,
      page: page
    }, function (err, res) {
      if ($this.pageLoad) {
        utils.loading(false);
        utils.scrollToTop();
      } else {
        $this.pageLoad = true;
      }

      if (err) {
        $this.pageAlert = {
          type: 'danger',
          html: err.message
        };
        return;
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