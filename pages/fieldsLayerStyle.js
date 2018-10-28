config.apiUrl = utils.getQueryString('apiUrl');
config.siteId = utils.getQueryString('siteId');
config.channelId = utils.getQueryString('channelId');
config.contentId = utils.getQueryString('contentId');
config.formId = utils.getQueryString('formId');
config.fieldId = utils.getQueryString('fieldId');
var $api = new utils.Api('/ss.form/fields/style');

var data = {
  pageLoad: false,
  pageAlert: null,
  styleInfo: null,
  isRapid: null,
  rapidValues: null,
};

var methods = {
  getStyle: function () {
    var $this = this;

    $api.get({
      siteId: config.siteId,
      channelId: config.channelId,
      contentId: config.contentId,
      formId: config.formId,
      fieldId: config.fieldId
    }, function (err, res) {
      $this.pageLoad = true;
      if (err || !res) return;

      $this.styleInfo = res.value;
      $this.isRapid = res.isRapid;
      $this.rapidValues = res.rapidValues;
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
          fieldId: config.fieldId,
          fieldInfo: $this.styleInfo,
          isRapid: $this.isRapid,
          rapidValues: $this.rapidValues,
        }, function (err, res) {
          utils.loading(false);
          if (err || !res) {
            $this.pageAlert = {
              type: 'danger',
              html: err.message
            }
            return;
          }

          parent.location.reload(true);
          utils.closeLayer();
        });
      }
    });
  },
  btnStyleItemRemoveClick: function (index) {
    this.styleInfo.items.splice(index, 1);
    if (this.styleInfo.items.length === 0) {
      this.btnStyleItemAddClick();
    }
  },
  btnStyleItemAddClick: function () {
    this.styleInfo.items.push({
      itemTitle: '',
      itemValue: '',
      isSelected: false
    })
  },
  btnRadioClick: function (index) {
    for (var i = 0; i < this.styleInfo.items.length; i++) {
      var element = this.styleInfo.items[i];
      element.isSelected = false;
    }
    this.styleInfo.items[index].isSelected = true;
  }
};

new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.getStyle();
  }
});