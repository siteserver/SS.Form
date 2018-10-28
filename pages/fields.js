config.apiUrl = utils.getQueryString('apiUrl');
config.siteId = utils.getQueryString('siteId');
config.channelId = utils.getQueryString('channelId');
config.contentId = utils.getQueryString('contentId');
config.formId = utils.getQueryString('formId');
config.returnUrl = utils.getQueryString('returnUrl');

var $api = new utils.Api('/ss.form/fields');

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

    $api.get({
      siteId: config.siteId,
      channelId: config.channelId,
      contentId: config.contentId,
      formId: config.formId
    }, function (err, res) {
      if (err || !res || !res.value) return;

      $this.items = res.value;
      $this.tableName = res.tableName;
      $this.relatedIdentities = res.relatedIdentities;

      $this.pageLoad = true;
    });
  },

  delete: function (fieldId) {
    var $this = this;

    utils.loading(true);
    $api.delete({
      siteId: config.siteId,
      channelId: config.channelId,
      contentId: config.contentId,
      formId: config.formId,
      fieldId: fieldId
    }, function (err, res) {
      utils.loading(false);
      if (err || !res || !res.value) return;

      $this.items = res.value;
    });
  },

  btnEditClick: function (fieldId) {
    utils.openLayer({
      title: '编辑字段',
      url: 'fieldsLayerStyle.html?siteId=' + config.siteId + '&formId=' + config.formId + '&fieldId=' + fieldId + '&apiUrl=' + encodeURIComponent(config.apiUrl)
    });
  },

  btnValidateClick: function (fieldId) {
    utils.openLayer({
      title: '设置验证规则',
      url: 'fieldsLayerValidate.html?siteId=' + config.siteId + '&channelId=' + config.channelId + '&contentId=' + config.contentId + '&formId=' + config.formId + '&fieldId=' + fieldId + '&apiUrl=' + encodeURIComponent(config.apiUrl)
    });
  },

  btnAddClick: function () {
    utils.openLayer({
      title: '新增字段',
      url: 'fieldsLayerStyle.html?siteId=' + config.siteId + '&channelId=' + config.channelId + '&contentId=' + config.contentId + '&formId=' + config.formId + '&apiUrl=' + encodeURIComponent(config.apiUrl)
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
    location.href = pageName + '?siteId=' + config.siteId + '&channelId=' + config.channelId + '&contentId=' + config.contentId + '&formId=' + config.formId + '&apiUrl=' + encodeURIComponent(config.apiUrl) + '&returnUrl=' + encodeURIComponent(config.returnUrl);
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