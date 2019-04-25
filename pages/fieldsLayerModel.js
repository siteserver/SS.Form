var $url = '/pages/fieldsLayerModel'
var $siteId = utils.getQueryString('siteId');
var $channelId = utils.getQueryString('channelId');
var $contentId = utils.getQueryString('contentId');
var $formId = utils.getQueryString('formId');
var $fieldId = utils.getQueryString('fieldId');

var data = {
  pageLoad: false,
  pageAlert: null,
  fieldInfo: null,
  isRapid: null,
  rapidValues: null
};

var methods = {
  getStyle: function () {
    var $this = this;

    $api.get($url, {
      params: {
        siteId: $siteId,
        channelId: $channelId,
        contentId: $contentId,
        formId: $formId,
        fieldId: $fieldId
      }
    }).then(function (response) {
      var res = response.data;

      $this.fieldInfo = res.value;
      $this.isRapid = res.isRapid;
      $this.rapidValues = res.rapidValues;
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
          fieldId: $fieldId,
          fieldInfo: $this.fieldInfo,
          items: $this.fieldInfo.items,
          isRapid: $this.isRapid,
          rapidValues: $this.rapidValues
        }).then(function (response) {
          var res = response.data;

          parent.location.reload(true);
          utils.closeLayer();
        }).catch(function (error) {
          $this.pageAlert = utils.getPageAlert(error);
        }).then(function () {
          utils.loading(false);
        });
      }
    });
  },

  btnItemInfoRemoveClick: function (index) {
    this.fieldInfo.items.splice(index, 1);
    if (this.fieldInfo.items.length === 0) {
      this.btnItemInfoAddClick();
    }
  },

  btnItemInfoAddClick: function () {
    this.fieldInfo.items.push({
      value: '',
      isSelected: false
    })
  },

  btnRadioClick: function (index) {
    for (var i = 0; i < this.fieldInfo.items.length; i++) {
      var element = this.fieldInfo.items[i];
      element.isSelected = false;
    }
    this.fieldInfo.items[index].isSelected = true;
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
