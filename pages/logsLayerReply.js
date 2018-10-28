config.apiUrl = utils.getQueryString('apiUrl');
config.siteId = utils.getQueryString('siteId');
config.channelId = utils.getQueryString('channelId');
config.contentId = utils.getQueryString('contentId');
config.formId = utils.getQueryString('formId');
config.logId = utils.getQueryString('logId');
var $api = new utils.Api('/ss.form/logs/reply');

var data = {
  pageLoad: false,
  pageAlert: null,
  logInfo: null,
  attributeNames: null
};

var methods = {
  load: function () {
    var $this = this;

    $api.get({
      siteId: config.siteId,
      channelId: config.channelId,
      contentId: config.contentId,
      formId: config.formId,
      logId: config.logId
    }, function (err, res) {
      $this.pageLoad = true;
      if (err || !res) return;

      $this.logInfo = res.value;
      $this.attributeNames = res.attributeNames;
    });
  },

  btnSubmitClick: function () {
    var $this = this;
    this.$validator.validate().then(function (result) {
      if (result) {
        utils.loading(true);
        $api.post({
          siteId: config.siteId,
          channelId: config.channelId,
          contentId: config.contentId,
          formId: config.formId,
          logId: config.logId,
          replyContent: $this.logInfo.replyContent,
        }, function (err, res) {
          utils.loading(false);
          if (err || !res) {
            $this.pageAlert = {
              type: 'danger',
              html: err.message
            }
            return;
          }

          alert({
            toast: true,
            type: 'success',
            title: "回复成功",
            showConfirmButton: false,
            timer: 1500
          }).then(function () {
            parent.location.reload(true);
            utils.closeLayer();
          });
        });
      }
    });
  },

  getAttributeText: function (attributeName) {
    if (attributeName === 'AddDate') {
      return '添加时间';
    } else if (attributeName === 'IsReplied') {
      return '是否回复';
    } else if (attributeName === 'ReplyDate') {
      return '回复时间';
    }

    return attributeName;
  },

  getAttributeValue: function (attributeName) {
    if (attributeName === 'IsReplied') {
      return this.logInfo.isReplied ? '<strong class="text-primary">已回复</strong>' : '<strong class="text-danger">未回复</strong>';
    } else if (attributeName === 'ReplyDate') {
      return this.logInfo.isReplied ? this.logInfo.replyDate : '';
    }

    return this.logInfo[_.camelCase(attributeName)];
  }
};

new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.load();
  }
});