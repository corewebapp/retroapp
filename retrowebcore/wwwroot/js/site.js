// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function addSpinner(node) {
    var spinner = document.createElement('span');
    spinner.classList.add('spinner');
    spinner.classList.add('tertiary');
    spinner.clientHeight = node.clientHeight;
    //spinner.style.float = 'left';
    node.append(spinner);
}

function openHrefWithPortal(link) {
    if ('HTMLPortalElement' in window) {
        var u = new URL(link);
        u.hash = "";
        var portal = document.createElement('portal');
        portal.addEventListener("load", e => {
            portal.activate();
        });
        portal.src = u.href;
        portal.style.display = 'none';
        document.body.appendChild(portal);
    } else
    {
        window.location = link;
    }
}

if ('HTMLPortalElement' in window) {
    var portal = document.createElement('portal');
    var portalClicked = false;
    document.querySelectorAll("a").forEach(function (each) {
        each.addEventListener("click", function (e) {
            e.preventDefault();
            addSpinner(each);
            openHrefWithPortal(e.target.href);
            
            if (window.portalHost) {
                window.portalHost.postMessage({ done: true, frm: location.pathname });
            }
        });
    });
} else {
    document.querySelectorAll("a").forEach(function (each) {
        each.addEventListener("click", function (e) {
            e.preventDefault();
            addSpinner(each);
            window.location = e.target.href;
        });
    });
}