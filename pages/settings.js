var $url = '/pages/settings';
var $apiUrl = utils.getQueryString('apiUrl');
var $siteId = utils.getQueryString('siteId');
var $channelId = utils.getQueryString('channelId');
var $contentId = utils.getQueryString('contentId');
var $formId = utils.getQueryString('formId');
var $returnUrl = utils.getQueryString('returnUrl');

var data = {
  pageConfig: null,
  pageLoad: false,
  pageAlert: null,
  pageType: 'list',
  formInfo: null,
  fieldInfoList: [],
  administratorSmsAttributeNames: null,
  administratorSmsNotifyKeys: null
};

var methods = {
  load: function () {
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

      $this.formInfo = res.value;
      $this.fieldInfoList = res.fieldInfoList;
      $this.administratorSmsAttributeNames = res.administratorSmsAttributeNames;
      $this.administratorSmsNotifyKeys = res.administratorSmsNotifyKeys;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      $this.pageLoad = true;
    });
  },

  submit: function () {
    var $this = this;

    var payload = {
      siteId: $siteId,
      channelId: $channelId,
      contentId: $contentId,
      formId: $formId,
      type: this.pageType
    };
    if (this.pageType === 'isClosed') {
      payload.isClosed = this.formInfo.additional.isClosed;
    } else if (this.pageType === 'title') {
      payload.title = this.formInfo.title;
    } else if (this.pageType === 'description') {
      payload.description = this.formInfo.description;
    } else if (this.pageType === 'isReply') {
      payload.isReply = this.formInfo.isReply;
    } else if (this.pageType === 'isTimeout') {
      payload.isTimeout = this.formInfo.additional.isTimeout;
      payload.timeToStart = this.formInfo.additional.timeToStart;
      payload.timeToEnd = this.formInfo.additional.timeToEnd;
    } else if (this.pageType === 'isCaptcha') {
      payload.isCaptcha = this.formInfo.additional.isCaptcha;
    } else if (this.pageType === 'isAdministratorSmsNotify') {
      payload.isAdministratorSmsNotify = this.formInfo.additional.isAdministratorSmsNotify;
      payload.administratorSmsNotifyTplId = this.formInfo.additional.administratorSmsNotifyTplId;
      payload.administratorSmsNotifyKeys = this.administratorSmsNotifyKeys.join(',');
      payload.administratorSmsNotifyMobile = this.formInfo.additional.administratorSmsNotifyMobile;
    } else if (this.pageType === 'isAdministratorMailNotify') {
      payload.isAdministratorMailNotify = this.formInfo.additional.isAdministratorMailNotify;
      payload.administratorMailNotifyAddress = this.formInfo.additional.administratorMailNotifyAddress;
    }

    utils.loading(true);
    $api.post($url, payload).then(function (response) {
      var res = response.data;

      $this.pageType = 'list';
      swal2({
        toast: true,
        type: 'success',
        title: "设置保存成功",
        showConfirmButton: false,
        timer: 2000
      });
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  },

  btnSubmitClick: function () {
    var $this = this;
    this.pageAlert = null;

    this.$validator.validate().then(function (result) {
      if (result) {
        $this.submit();
      }
    });
  },

  getAttributeText: function (attributeName) {
    if (attributeName === 'AddDate') {
      return '添加时间';
    }
    return attributeName;
  },

  btnNavClick: function (pageName) {
    location.href = pageName + '?siteId=' + $siteId + '&channelId=' + $channelId + '&contentId=' + $contentId + '&formId=' + $formId + '&apiUrl=' + encodeURIComponent($apiUrl) + '&returnUrl=' + encodeURIComponent($returnUrl);
  }
};

Vue.component("date-picker", window.DatePicker.default);

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function () {
    this.load();
  }
});
