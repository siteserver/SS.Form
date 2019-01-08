var $url = '/pages/logsLayerReply';
var $apiUrl = utils.getQueryString('apiUrl');
var $siteId = utils.getQueryString('siteId');
var $channelId = utils.getQueryString('channelId');
var $contentId = utils.getQueryString('contentId');
var $formId = utils.getQueryString('formId');
var $logId = utils.getQueryString('logId');

var data = {
  pageLoad: false,
  pageAlert: null,
  logInfo: null,
  attributeNames: null
};

var methods = {
  load: function () {
    var $this = this;

    $api.get($url, {
      params: {
        siteId: $siteId,
        channelId: $channelId,
        contentId: $contentId,
        formId: $formId,
        logId: $logId
      }
    }).then(function (response) {
      var res = response.data;

      $this.logInfo = res.value;
      $this.attributeNames = res.attributeNames;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      $this.pageLoad = true;
    });
  },

  btnSubmitClick: function () {
    var $this = this;
    this.$validator.validate().then(function (result) {
      if (result) {
        utils.loading(true);

        $api.post($url, {
          siteId: $siteId,
          channelId: $channelId,
          contentId: $contentId,
          formId: $formId,
          logId: $logId,
          replyContent: $this.logInfo.replyContent
        }).then(function (response) {
          var res = response.data;

          swal2({
            toast: true,
            type: 'success',
            title: "回复成功",
            showConfirmButton: false,
            timer: 1500
          }).then(function () {
            parent.location.reload(true);
            utils.closeLayer();
          });
        }).catch(function (error) {
          $this.pageAlert = utils.getPageAlert(error);
        }).then(function () {
          utils.loading(false);
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