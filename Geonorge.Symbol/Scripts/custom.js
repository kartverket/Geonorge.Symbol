var organizations = [];

$('.ownerOrganizationSelect').select2({
    placeholder: "Søk etter organisasjon",
    language: "nb",
    ajax: {
        url: registryUrl + "api/search",
        dataType: 'json',
        delay: 250,
        data: function (params) {
            return {
                text: params.term,// search term
                limit: 10,
                'facets[0]name': "type",
                'facets[0]value': "organisasjoner"
            };
        },
        processResults: function (data, params) {
            organizations = [];
            $.each(data.Results, function (i, item) {
                option = {}
                option["id"] = item.Name;
                option["text"] = item.Name;

                organizations.push(option);
            })

            return {
                results: organizations
            };
        },
        cache: true
    },
    minimumInputLength: 3
});

$(".packagesSelect").select2();