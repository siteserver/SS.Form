var $url = '/pages/fields';

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
  items: null,
  tableName: null,
  relatedIdentities: null
};

var methods = {
  getList: function () {
    var $this = this;

    $api.get($url, {
      params: {
        siteId: $siteId,
        channelId: $channelId,
        contentId: $contentId,
        formId: $formId
      }
    }).then(function (response) {
      var res = response.data;

      $this.items = res.value;
      $this.tableName = res.tableName;
      $this.relatedIdentities = res.relatedIdentities;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      $this.pageLoad = true;
    });
  },

  delete: function (fieldId) {
    var $this = this;

    utils.loading(true);
    $api.delete($url, {
      params: {
        siteId: $siteId,
        channelId: $channelId,
        contentId: $contentId,
        formId: $formId,
        fieldId: fieldId
      }
    }).then(function (response) {
      var res = response.data;

      $this.items = res.value;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  },

  btnEditClick: function (fieldId) {
    utils.openLayer({
      title: '编辑字段',
      url: 'fieldsLayerStyle.html?siteId=' + $siteId + '&formId=' + $formId + '&fieldId=' + fieldId + '&apiUrl=' + encodeURIComponent($apiUrl)
    });
  },

  btnValidateClick: function (fieldId) {
    utils.openLayer({
      title: '设置验证规则',
      url: 'fieldsLayerValidate.html?siteId=' + $siteId + '&channelId=' + $channelId + '&contentId=' + $contentId + '&formId=' + $formId + '&fieldId=' + fieldId + '&apiUrl=' + encodeURIComponent($apiUrl)
    });
  },

  btnAddClick: function () {
    utils.openLayer({
      title: '新增字段',
      url: 'fieldsLayerStyle.html?siteId=' + $siteId + '&channelId=' + $channelId + '&contentId=' + $contentId + '&formId=' + $formId + '&apiUrl=' + encodeURIComponent($apiUrl)
    });
  },

  btnDeleteClick: function (title, fieldId) {
    var $this = this;

    utils.alertDelete({
      title: '删除字段',
      text: '此操作将删除字段 ' + title + '，确定吗？',
      callback: function () {
        $this.delete(fieldId);
      }
    });
  },

  btnNavClick: function (pageName) {
    location.href = pageName + '?siteId=' + $siteId + '&channelId=' + $channelId + '&contentId=' + $contentId + '&formId=' + $formId + '&apiUrl=' + encodeURIComponent($apiUrl) + '&returnUrl=' + encodeURIComponent($returnUrl);
  }
};

new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.getList();
  }
});