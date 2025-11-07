import { jsx as _jsx, jsxs as _jsxs } from "react/jsx-runtime";
import { Box, Flex, IconButton } from "@radix-ui/themes";
import { useNavigate } from "react-router-dom";
import ProfileButton from "./ProfileButton";
import { ChevronLeftIcon } from "lucide-react";
function Header() {
    const navigate = useNavigate();
    return (_jsx(Box, { style: {
            padding: "10px"
        }, children: _jsxs(Flex, { style: {
                alignItems: "center",
                justifyContent: "space-between"
            }, children: [_jsx(IconButton, { variant: "surface", onClick: () => navigate(-1), style: { backgroundColor: "transparent", border: "none", cursor: "pointer" }, children: _jsx(ChevronLeftIcon, {}) }), _jsx(Flex, { style: { gap: "10px" }, children: _jsx(ProfileButton, {}) })] }) }));
}
export default Header;
