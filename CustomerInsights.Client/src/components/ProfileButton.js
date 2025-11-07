import { jsx as _jsx, jsxs as _jsxs } from "react/jsx-runtime";
import { Avatar, DropdownMenu, Flex, Strong, Text } from "@radix-ui/themes";
import { useNavigate } from "react-router-dom";
import { CaretDownIcon, CaretUpIcon } from "@radix-ui/react-icons";
function ProfileButton() {
    const navigate = useNavigate();
    function handleLogout() {
        navigate("/login");
    }
    return (_jsxs(DropdownMenu.Root, { children: [_jsx(DropdownMenu.Trigger, { children: _jsxs(Flex, { direction: "row", gap: "3", style: { cursor: "pointer" }, children: [_jsx(Flex, { direction: "column", style: { justifyContent: "center" }, children: _jsx(Avatar, { fallback: "M", style: { width: "35px", height: "35px" } }) }), _jsxs(Flex, { direction: "column", style: { justifyContent: "center" }, children: [_jsx(Text, { size: "2", children: _jsx(Strong, { children: "Max Mustermann" }) }), _jsx(Text, { size: "1", children: "max.mustermann@test-gmbh.com" })] }), _jsxs(Flex, { direction: "column", style: { justifyContent: "center" }, children: [_jsx(CaretUpIcon, { width: "20", height: "20" }), _jsx(CaretDownIcon, { width: "20", height: "20" })] })] }) }), _jsxs(DropdownMenu.Content, { color: "gray", highContrast: true, style: { width: "269px" }, children: [_jsx(DropdownMenu.Item, { style: { cursor: "pointer" }, onClick: () => navigate("/profile"), children: "Profile" }), _jsx(DropdownMenu.Item, { style: { cursor: "pointer" }, onClick: () => navigate("/admin/users"), children: "Settings" }), _jsx(DropdownMenu.Separator, {}), _jsx(DropdownMenu.Item, { style: { cursor: "pointer" }, onClick: () => handleLogout(), children: "Logout" })] })] }));
}
export default ProfileButton;
